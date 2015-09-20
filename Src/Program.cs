using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using RT.Servers;
using RT.TagSoup;
using RT.Util;
using RT.Util.CommandLine;
using RT.Util.Consoles;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;
using RT.Util.Json;
using RT.Util.Serialization;
using RT.Util.Text;

namespace QuizGameEngine
{
    static class Program
    {
        public static QuizBase Quiz;
        public static HttpServer Server;
        public static HashSet<QuizWebSocket> Sockets = new HashSet<QuizWebSocket>();
        private static string _dataFile;
        private static string _logFile;

        public static void LogMessage(string msg)
        {
            if (_logFile != null)
                lock (_logFile)
                    File.AppendAllText(_logFile, Environment.NewLine + Environment.NewLine + msg);
        }

        public static ConsoleKeyInfo ReadKey()
        {
            ConsoleUtil.Write("<press a key>".Color(ConsoleColor.DarkYellow));
            var ret = Console.ReadKey(true);
            Console.WriteLine();
            return ret;
        }

        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "--post-build-check")
                return Ut.RunPostBuildChecks(args[1], Assembly.GetExecutingAssembly());

            try
            {
                Console.OutputEncoding = Encoding.UTF8;
                Console.TreatControlCAsInput = true;
            }
            catch { }

            CommandLine cmd;
            try
            {
                cmd = CommandLineParser.Parse<CommandLine>(args);
            }
            catch (CommandLineParseException e)
            {
                e.WriteUsageInfoToConsole();
                return 1;
            }

            var start = cmd.QuizCmd as QuizCmdStart;
            if (start != null)
            {
                ClassifyJson.SerializeToFile(start.Quiz.StartState, start.OutputFile);
                ConsoleUtil.WriteLine("File saved.");
                return 0;
            }

            _dataFile = ((QuizCmdLoad) cmd.QuizCmd).File;
            Quiz = ClassifyJson.DeserializeFile<QuizBase>(_dataFile);
            _logFile = cmd.LogFile;

            var getResourceHandler = Ut.Lambda((Func<byte[], HttpResponse> function, byte[] resource, string filename) =>
            {
                return Ut.Lambda((HttpRequest req) =>
                {
                    if (cmd.ResourcePath == null)
                        return function(resource);
                    return function(File.ReadAllBytes(Path.Combine(cmd.ResourcePath, filename)));
                });
            });

            Server = new HttpServer(24567);
            var resolver = new UrlResolver(
                new UrlMapping(path: "/", specificPath: true, handler: handle),
                new UrlMapping(path: "/jquery", specificPath: true, handler: getResourceHandler(d => HttpResponse.JavaScript(d), Resources.JQuery, "jquery-2.1.4.js")),
                new UrlMapping(path: "/js1", specificPath: true, handler: getResourceHandler(d => HttpResponse.JavaScript(d), Quiz.Js, Quiz.CssJsFilename + ".js")),
                new UrlMapping(path: "/js2", specificPath: true, handler: getResourceHandler(d => HttpResponse.JavaScript(d), Resources.GlobalJs, "Global.js")),
                new UrlMapping(path: "/css", specificPath: true, handler: getResourceHandler(d => HttpResponse.Css(d), Quiz.Css, Quiz.CssJsFilename + ".css")),
                new UrlMapping(path: "/socket", specificPath: true, handler: webSocket));
            if (cmd.ResourcePath != null)
            {
                var fileResolver = new FileSystemHandler(Path.Combine(cmd.ResourcePath, Quiz.CssJsFilename));
                resolver.Add(new UrlMapping(path: "/f", handler: fileResolver.Handle));
            }
            Server.Handler = resolver.Handle;
            Server.StartListening();

            var needSave = false;
            while (true)
            {
                Console.Clear();
                ConsoleUtil.WriteLine(Quiz.UndoLine == null ? null : "← Undo: {0/Gray}".Color(ConsoleColor.Magenta).Fmt(Quiz.UndoLine));
                ConsoleUtil.WriteLine(Quiz.RedoLine == null ? null : "→ Redo: {0/Gray}".Color(ConsoleColor.Green).Fmt(Quiz.RedoLine));
                ConsoleUtil.WriteLine(Quiz.CurrentState.GetType().Name.Color(ConsoleColor.DarkYellow), align: HorizontalTextAlignment.Right);
                ConsoleUtil.WriteLine(Quiz.CurrentState.Describe);
                Console.WriteLine();

                var keysUsed = new Dictionary<ConsoleKey, int>();
                foreach (var t in Quiz.CurrentState.Transitions)
                {
                    keysUsed.IncSafe(t.Key);
                    if (keysUsed[t.Key] > 1)
                        ConsoleUtil.WriteLine("Key {0/Magenta} used more than once.".Color(ConsoleColor.Red).Fmt(t.Key));
                    else if (t.Key == ConsoleKey.Escape || t.Key == ConsoleKey.Backspace)
                        ConsoleUtil.WriteLine("Key {0/Magenta} is already bound.".Color(ConsoleColor.Red).Fmt(t.Key));
                    ConsoleUtil.WriteLine("{0/White}: {1/Cyan}".Color(null).Fmt(t.Key, t.Name));
                }

                again:
                if (needSave)
                {
                    save();
                    needSave = false;
                }

                var keyInfo = Console.ReadKey(true);
                TransitionResult transitionResult = null;
                string transitionUndoLine = null;

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        if (keyInfo.Modifiers != 0)
                            goto default;
                        return 0;

                    case ConsoleKey.Backspace:
                        if (
                            (keyInfo.Modifiers == ConsoleModifiers.Shift) ? Quiz.Redo() :
                            (keyInfo.Modifiers == 0) ? Quiz.Undo() :
                            false)
                            needSave = true;
                        break;

                    case ConsoleKey.F5:
                        if (keyInfo.Modifiers != 0)
                            goto default;
                        Quiz = ClassifyJson.DeserializeFile<QuizBase>(_dataFile);
                        needSave = true;
                        transitionResult = new TransitionResult(Quiz.CurrentState, null, Quiz.CurrentState.JsMethod, Quiz.CurrentState.JsParameters);
                        break;

                    case ConsoleKey.E:
                        if (keyInfo.Modifiers != 0)
                            goto default;
                        var newState = Quiz.CurrentState;
                        Edit(newState, new[] { "Quiz" }, setValue: val => { newState = (QuizStateBase) val; });
                        needSave = true;
                        transitionResult = new TransitionResult(newState);
                        transitionUndoLine = "Edit quiz state manually";
                        break;

                    default:
                        var transition = Quiz.CurrentState.Transitions.FirstOrDefault(q => q.Key == keyInfo.Key);
                        if (transition == null)
                            goto again;

                        Console.WriteLine();    // In case transition.Execute() outputs something and then waits for a key
                        transitionResult = transition.Execute();
                        transitionUndoLine = transition.Name;
                        break;
                }

                if (transitionResult != null)
                {
                    if (transitionResult.State != null && transitionUndoLine != null)
                        Quiz.Transition(transitionResult.State, transitionUndoLine);

                    if (transitionResult.JsMethod != null)
                    {
                        var json = ClassifyJson.Serialize(transitionResult.JsParameters);
                        lock (Sockets)
                            foreach (var socket in Sockets)
                                socket.SendLoggedMessage(new JsonDict { { "method", transitionResult.JsMethod }, { "params", json } });
                    }

                    needSave = true;
                }
            }
        }

        private static void save()
        {
            ClassifyJson.SerializeToFile(Quiz, _dataFile);
        }

        private static HttpResponse handle(HttpRequest req)
        {
            LogMessage("{0} request for {1} from {2}".Fmt(req.Method, req.Url.ToHref(), req.SourceIP));
            return HttpResponse.Html(
                new HTML(
                    new HEAD(
                        new TITLE("Quiz"),
                        new META { httpEquiv = "Content-type", content = "text/html; charset=utf-8" },
                        new SCRIPT { src = "/jquery" },
                        new LINK { rel = "stylesheet", href = "/css" },
                        new SCRIPT { src = "/js1" },
                        new SCRIPT { src = "/js2" }),
                    new BODY().Data("socket-url", Regex.Replace(req.Url.WithParent("socket").ToFull(), @"^http", "ws"))._(new DIV { id = "content" })));
        }

        private static HttpResponse webSocket(HttpRequest req)
        {
            return HttpResponse.WebSocket(new QuizWebSocket(req.SourceIP));
        }

        public static void Edit(dynamic obj, string[] path, Type type = null, Action<object> setValue = null, string promptForPrimitiveValue = null)
        {
            var isNullable = type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            var nullableElementType = isNullable ? type.GetGenericArguments()[0] : null;

            if (type == null || (obj != null && !isNullable))
                type = ((object) obj).GetType();
            var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            var genericArguments = type.IsGenericType ? type.GetGenericArguments() : null;

            var copyEnsured = false;
            var ensureCopy = Ut.Lambda(() =>
            {
                if (copyEnsured)
                    return;
                copyEnsured = true;
                if (obj is Array)
                {
                    var arr = (Array) obj;
                    var newObj = Array.CreateInstance(type.GetElementType(), arr.Length);
                    Array.Copy(obj, newObj, arr.Length);
                    obj = newObj;
                    setValue(obj);
                }
                else if (genericType == typeof(List<>))
                {
                    obj = Activator.CreateInstance(typeof(List<>).MakeGenericType(genericArguments), new object[] { obj });
                    setValue(obj);
                }
                else if (genericType == typeof(Dictionary<,>))
                {
                    obj = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(genericArguments), new object[] { obj });
                    setValue(obj);
                }
                else if (obj is ICloneable)
                {
                    obj = ((ICloneable) obj).Clone();
                    setValue(obj);
                }
            });

            if (ExactConvert.IsSupportedType(type) || (isNullable && ExactConvert.IsSupportedType(nullableElementType)))
            {
                object result;
                var input = ExactConvert.ToString(((object) obj) ?? "");
                while (true)
                {
                    input = InputBox.GetLine(promptForPrimitiveValue ?? "Type new value:", input);
                    if (input == null)
                        return;

                    if (input == "" && isNullable)
                        input = null;
                    else if (input == "" && type == typeof(string))
                    {
                        var dlgResult = DlgMessage.Show("Null or empty?", "Confirmation", DlgType.Question, "&Null", "&Empty", "&Cancel");
                        if (dlgResult == 2)
                            return;
                        if (dlgResult == 0)
                            input = null;
                    }

                    try
                    {
                        result = input.NullOr(i => ExactConvert.To(isNullable ? nullableElementType : type, i));
                        setValue(result);
                        return;
                    }
                    catch (Exception e)
                    {
                        DlgMessage.Show(e.Message, DlgType.Error);
                    }
                }
            }

            var cursorVisible = Console.CursorVisible;
            Console.CursorVisible = false;

            path = path ?? new[] { "Data" };

            var selStart = 0;
            var selLength = 1;
            var selIsTop = false;

            var getEditables = Ut.Lambda((IEnumerable<FieldInfo> fields) => fields.Select(f =>
            {
                MemberInfo getAttrsFrom = f;
                var rFieldName = f.Name;

                if (rFieldName.StartsWith("<") && rFieldName.EndsWith(">k__BackingField"))
                {
                    // Compiler-generated fields for auto-implemented properties 
                    rFieldName = rFieldName.Substring(1, rFieldName.Length - "<>k__BackingField".Length);
                    var prop = type.GetAllProperties().FirstOrDefault(p => p.Name == rFieldName);
                    if (prop != null)
                        getAttrsFrom = prop;
                }

                var attr = getAttrsFrom.GetCustomAttribute<EditorLabelAttribute>();

                return new
                {
                    Label = attr == null ? rFieldName : attr.Label,
                    DeclaredType = f.FieldType,
                    GetValue = new Func<dynamic>(() => f.GetValue((object) obj)),
                    SetValue = new Action<dynamic>(val => { ensureCopy(); f.SetValue((object) obj, val); }),
                    Key = (dynamic) null
                };
            }).ToArray());

            // Dummy value; will be overwritten
            var editables = getEditables(Enumerable.Empty<FieldInfo>());

            while (true)
            {
                if (editables.Length == 0)
                    selStart = 0;
                else
                    selStart = selStart.Clip(0, editables.Length - 1);

                var isCollection = false;
                var isDictionary = false;
                Type keyType = null, valueType = null;

                if (genericType == typeof(Dictionary<,>))
                {
                    isDictionary = true;
                    keyType = genericArguments[0];
                    valueType = genericArguments[1];

                    editables = ((IEnumerable) obj).Cast<dynamic>().Select(kvp => new
                    {
                        Label = (string) kvp.Key.ToString(),
                        DeclaredType = valueType,
                        GetValue = new Func<dynamic>(() => kvp.Value),
                        SetValue = new Action<dynamic>(val => { ensureCopy(); obj[kvp.Key] = val; }),
                        Key = kvp.Key
                    }).ToArray();
                }
                else if (genericType == typeof(List<>) || type.IsArray)
                {
                    isCollection = true;
                    valueType = type.IsArray ? type.GetElementType() : genericArguments[0];

                    editables = ((IEnumerable) obj).Cast<dynamic>().Select((elem, index) => new
                    {
                        Label = index.ToString(),
                        DeclaredType = valueType,
                        GetValue = new Func<dynamic>(() => elem),
                        SetValue = new Action<dynamic>(val => { ensureCopy(); obj[index] = val; }),
                        Key = (dynamic) index
                    }).ToArray();
                }
                else
                    editables = getEditables(type.GetAllFields());

                Console.Clear();
                ConsoleUtil.WriteLine(path.Select(p => p.Color(ConsoleColor.White)).JoinColoredString(" ▶ ".Color(ConsoleColor.Green)).ColorBackground(ConsoleColor.DarkGreen));
                ConsoleUtil.WriteLine(type.FullName.Color(ConsoleColor.DarkGreen));
                if (genericType != typeof(Dictionary<,>) && genericType != typeof(List<>) && !(obj is ICloneable) && !(obj is Array))
                    ConsoleUtil.WriteLine("NOT CLONEABLE".Color(ConsoleColor.Yellow, ConsoleColor.Red));
                Console.WriteLine();

                var t = new TextTable { ColumnSpacing = 2 };
                for (int i = 0; i < editables.Length; i++)
                {
                    var editable = editables[i];
                    var value = (object) editable.GetValue();
                    var selected = i >= selStart && i < selStart + selLength;

                    if (i == (selIsTop ? selStart : selStart + selLength - 1))
                        t.SetCell(0, i, "▶");
                    t.SetCell(1, i, editable.Label);
                    t.SetCell(2, i, value == null ? "<null>".Color(ConsoleColor.DarkGray) : value.ToUsefulString());
                    if (selected)
                        t.SetRowBackground(i, ConsoleColor.DarkBlue);
                }
                t.WriteToConsole();

                var addElement = Ut.Lambda((int index) =>
                {
                    object key = null;
                    if (isDictionary)
                    {
                        while (true)
                        {
                            Edit(Activator.CreateInstance(keyType), path.Concat("new key").ToArray(), keyType, val => { key = val; }, "Enter new dictionary key:");
                            if (key == null)
                                return;
                            if (!obj.ContainsKey((dynamic) key))
                                break;
                            DlgMessage.Show("The key is already in the dictionary.", DlgType.Error);
                        }
                    }
                    else
                        key = index;

                    dynamic value = createInstance(valueType);
                    if (value == null && valueType != typeof(string))
                        return;
                    Edit((object) value, path.Concat(key.ToString()).ToArray(), valueType, val => { value = val; }, "Enter new value:");

                    ensureCopy();
                    if (isDictionary)
                        obj.Add((dynamic) key, value);
                    else if (!type.IsArray)
                        obj.Add(value);
                    else
                    {
                        obj = Extensions.InsertAtIndex(obj, index, value);
                        setValue(obj);
                    }
                });

                var selectedElements = Ut.Lambda((bool delete) =>
                {
                    object[] ret;
                    if (isDictionary)
                    {
                        ret = Enumerable.Range(selStart, selLength).Select(i => (object) Ut.KeyValuePair((object) editables[i].Key, (object) obj[editables[i].Key])).ToArray();
                        if (delete)
                        {
                            ensureCopy();
                            for (int i = 0; i < selLength; i++)
                                obj.Remove(editables[selStart + i].Key);
                        }
                    }
                    else if (type.IsArray)
                    {
                        ret = Enumerable.Range(selStart, selLength).Select(i => obj[i]).ToArray();
                        if (delete)
                        {
                            copyEnsured = true;
                            obj = Extensions.RemoveIndexes(obj, selStart, selLength);
                            setValue(obj);
                        }
                    }
                    else
                    {
                        ret = Enumerable.Range(selStart, selLength).Select(i => obj[i]).ToArray();
                        if (delete)
                        {
                            ensureCopy();
                            for (int i = 0; i < selLength; i++)
                                obj.RemoveAt(selStart);
                        }
                    }
                    return ret;
                });

                var keyInfo = Console.ReadKey(true);
                if ((keyInfo.Key == ConsoleKey.Escape || keyInfo.Key == ConsoleKey.Backspace) && keyInfo.Modifiers == 0)
                    break;

                bool insertAtBottom = false;

                switch (KeyName(keyInfo.Key, keyInfo.Modifiers).ToString())
                {
                    case "UpArrow":
                        if (selStart > 0)
                            selStart--;
                        selLength = 1;
                        break;

                    case "DownArrow":
                        if (selStart + selLength < editables.Length)
                            selStart = selStart + selLength;
                        else
                            selStart = editables.Length - 1;
                        selLength = 1;
                        break;

                    case "Home":
                        selStart = 0;
                        selLength = 1;
                        break;

                    case "End":
                        selStart = editables.Length - 1;
                        selLength = 1;
                        break;

                    case "Shift+UpArrow":
                        if (selLength == 1 || selIsTop)
                        {
                            if (selStart == 0)
                                break;
                            selIsTop = true;
                            selStart--;
                            selLength++;
                        }
                        else
                            selLength--;
                        break;

                    case "Shift+DownArrow":
                        if (selLength == 1 || !selIsTop)
                        {
                            if (selStart + selLength == editables.Length)
                                break;
                            selLength++;
                            selIsTop = false;
                        }
                        else
                        {
                            selStart++;
                            selLength--;
                        }
                        break;

                    case "Shift+Home":
                        if (selIsTop || selLength == 1)
                            selLength += selStart;
                        else
                            selLength = selStart + 1;
                        selIsTop = true;
                        selStart = 0;
                        break;

                    case "Shift+End":
                        if (selIsTop && selLength > 1)
                            selStart = selStart + selLength - 1;
                        selIsTop = false;
                        selLength = editables.Length - selStart;
                        break;

                    case "Ctrl+A":
                        selStart = 0;
                        selLength = editables.Length;
                        selIsTop = false;
                        break;

                    case "Shift+OemPlus":
                        if (selLength == 1 && ExactConvert.IsTrueIntegerType(editables[selStart].DeclaredType))
                            editables[selStart].SetValue(unchecked(editables[selStart].GetValue() + 1));
                        break;

                    case "OemMinus":
                        if (selLength == 1 && ExactConvert.IsTrueIntegerType(editables[selStart].DeclaredType))
                            editables[selStart].SetValue(unchecked(editables[selStart].GetValue() - 1));
                        break;

                    case "Enter":
                        if (selLength != 1)
                            break;
                        var value = editables[selStart].GetValue();
                        if (value is bool)
                            editables[selStart].SetValue(!((bool) value));
                        else
                            Edit(value, path.Concat(editables[selStart].Label).ToArray(), editables[selStart].DeclaredType, editables[selStart].SetValue);
                        break;

                    case "A":
                        if (isCollection || isDictionary)
                            addElement(editables.Length);
                        break;

                    case "Insert":
                        if ((isCollection || isDictionary) && selLength == 1)
                            addElement(selStart);
                        break;

                    case "Ctrl+X":
                    case "Shift+Delete":
                        if (isCollection || isDictionary)
                            Clipboard.SetText(ClassifyJson.Serialize<object[]>(selectedElements(true)).ToStringIndented());
                        break;

                    case "Ctrl+C":
                    case "Ctrl+Insert":
                        if (isCollection || isDictionary)
                            Clipboard.SetText(ClassifyJson.Serialize<object[]>(selectedElements(false)).ToStringIndented());
                        break;

                    case "Ctrl+Alt+V":
                    case "Shift+Alt+Insert":
                        insertAtBottom = true;
                        goto case "Ctrl+V";

                    case "Ctrl+V":
                    case "Shift+Insert":
                        if ((isCollection || isDictionary) && selLength == 1)
                        {
                            try
                            {
                                var arr = ClassifyJson.Deserialize<object[]>(JsonValue.Parse(Clipboard.GetText()));
                                var index = insertAtBottom ? (type.IsArray ? obj.Length : obj.Count) : selStart;
                                if (isDictionary)
                                {
                                    if (arr.Any(o => getCompatibleKeyValuePair(keyType, valueType, o) == null))
                                        goto cannot;
                                }
                                else // isCollection
                                {
                                    if (arr.Any(o => !valueType.IsAssignableFrom(o.GetType())))
                                        goto cannot;
                                }

                                foreach (dynamic o in arr)
                                {
                                    if (isDictionary)
                                        obj.Add(o.Key, o.Value);
                                    else if (type.IsArray)
                                    {
                                        obj = Extensions.InsertAtIndex(obj, index, o);
                                        setValue(obj);
                                        index++;
                                    }
                                    else // isCollection
                                    {
                                        obj.Insert(index, o);
                                        index++;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                DlgMessage.Show(e.Message, DlgType.Error);
                            }
                        }
                        break;

                        cannot:
                        DlgMessage.Show("The value(s) from the clipboard cannot be used in this context.", DlgType.Error);
                        break;
                }
            }

            Console.Clear();
            Console.CursorVisible = cursorVisible;
        }

        private static object getCompatibleKeyValuePair(Type keyType, Type valueType, object kvp)
        {
            var type = kvp.GetType();
            if (!type.IsGenericType)
                return null;
            if (type.GetGenericTypeDefinition() != typeof(KeyValuePair<,>))
                return null;
            var args = type.GetGenericArguments();
            if (!keyType.IsAssignableFrom(args[0]) || !valueType.IsAssignableFrom(args[1]))
                return null;
            dynamic kvpDyn = kvp;
            return Activator.CreateInstance(typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType), new object[] { kvpDyn.Key, kvpDyn.Value });
        }

        public static ConsoleColoredString KeyName(ConsoleKey key, ConsoleModifiers modifiers)
        {
            var parts = new List<ConsoleColoredString>();
            if (modifiers.HasFlag(ConsoleModifiers.Control))
                parts.Add("Ctrl".Color(ConsoleColor.Yellow));
            if (modifiers.HasFlag(ConsoleModifiers.Alt))
                parts.Add("Alt".Color(ConsoleColor.Yellow));
            if (modifiers.HasFlag(ConsoleModifiers.Shift))
                parts.Add("Shift".Color(ConsoleColor.Yellow));
            parts.Add(key.ToString().Color(ConsoleColor.Yellow));
            return parts.JoinColoredString("+".Color(ConsoleColor.DarkYellow));
        }

        private static object createInstance(Type type)
        {
            if (type.IsArray)
                return Array.CreateInstance(type.GetElementType(), 0);

            if (type == typeof(string))
                return null;

            if (type.IsAbstract)
            {
                Console.Clear();
                var availableTypes = type.Assembly.GetTypes().Where(t => !t.IsAbstract && type.IsAssignableFrom(t)).ToArray();
                type = ConsoleSelect(availableTypes, t => t.Name);
                if (type == null)
                    return null;
            }

            return Activator.CreateInstance(type, true);
        }

        public static object GetConsoleSelection(Dictionary<ConsoleKey, Tuple<ConsoleColoredString, object>> selections)
        {
            Console.WriteLine();
            var keysUsed = new HashSet<ConsoleKey>();

            foreach (var kvp in selections)
            {
                if (!keysUsed.Add(kvp.Key))
                    ConsoleUtil.WriteLine("Key {0/Magenta} used more than once.".Color(ConsoleColor.Red).Fmt(kvp.Key));
                ConsoleUtil.WriteLine("{0/White}: {1}".Color(null).Fmt(kvp.Key, kvp.Value.Item1));
            }

            Tuple<ConsoleColoredString, object> ret;
            return selections.TryGetValue(Program.ReadKey().Key, out ret) ? ret.Item2 : null;
        }

        public static object GetConsoleSelectionFull(Dictionary<ConsoleKey, Tuple<ConsoleColoredString, object>> selections)
        {
            object cur = selections;
            while (true)
            {
                var curDic = cur as Dictionary<ConsoleKey, Tuple<ConsoleColoredString, object>>;
                if (curDic == null)
                    return cur;
                cur = GetConsoleSelection(curDic);
            }
        }

        public static T ConsoleSelect<T>(this IEnumerable<T> objs, Func<T, ConsoleColoredString> describe)
        {
            var dic = new Dictionary<ConsoleKey, Tuple<ConsoleColoredString, object>>();
            var curDic = dic;
            var curKey = ConsoleKey.A;
            foreach (var obj in objs)
            {
                if (curKey == ConsoleKey.Z)
                {
                    var newDic = new Dictionary<ConsoleKey, Tuple<ConsoleColoredString, object>>();
                    curDic[curKey] = new Tuple<ConsoleColoredString, object>("more...".Color(ConsoleColor.Magenta), newDic);
                    curDic = newDic;
                    curKey = ConsoleKey.A;
                }

                curDic[curKey] = new Tuple<ConsoleColoredString, object>(describe(obj), obj);
                curKey++;
            }
            return (T) GetConsoleSelectionFull(dic);
        }
    }
}