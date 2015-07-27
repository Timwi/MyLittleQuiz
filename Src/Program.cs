using System;
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
using RT.Util.ExtensionMethods;
using RT.Util.Json;
using RT.Util.Serialization;

namespace QuizGameEngine
{
    static class Program
    {
        public static QuizBase Quiz;
        public static HttpServer Server;
        public static HashSet<QuizWebSocket> Sockets = new HashSet<QuizWebSocket>();

        public static ConsoleKeyInfo ReadKey()
        {
            ConsoleUtil.Write("<press a key>".Color(ConsoleColor.DarkYellow));
            return Console.ReadKey(true);
        }

        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "--post-build-check")
                return Ut.RunPostBuildChecks(args[1], Assembly.GetExecutingAssembly());

            try { Console.OutputEncoding = Encoding.UTF8; }
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

            var file = ((QuizCmdLoad) cmd.QuizCmd).File;
            Quiz = ClassifyJson.DeserializeFile<QuizBase>(file);

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
            Server.Handler = resolver.Handle;
            Server.StartListening();

            while (true)
            {
                Console.Clear();
                if (Quiz.UndoLine != null)
                    ConsoleUtil.WriteLine("← Undo: {0/Gray}".Color(ConsoleColor.Magenta).Fmt(Quiz.UndoLine));
                if (Quiz.RedoLine != null)
                    ConsoleUtil.WriteLine("→ Redo: {0/Gray}".Color(ConsoleColor.Green).Fmt(Quiz.RedoLine));
                Console.WriteLine();
                ConsoleUtil.WriteLine(Quiz.CurrentState.Describe);
                Console.WriteLine();
                foreach (var t in Quiz.CurrentState.Transitions)
                    ConsoleUtil.WriteLine("{0/White}: {1/Cyan}".Color(null).Fmt(t.Key, t.Name));

                again:
                var keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        goto exit;

                    case ConsoleKey.Backspace:
                        if (keyInfo.Modifiers == ConsoleModifiers.Shift)
                            Quiz.Redo();
                        else if (keyInfo.Modifiers == 0)
                            Quiz.Undo();
                        else
                            goto default;
                        break;

                    default:
                        var transition = Quiz.CurrentState.Transitions.FirstOrDefault(q => q.Key == keyInfo.Key);
                        TransitionResult transitionResult;
                        if (transition == null)
                        {
                            if (keyInfo.Key == ConsoleKey.F5 && keyInfo.Modifiers == 0)
                            {
                                Quiz = ClassifyJson.DeserializeFile<QuizBase>(file);
                                transitionResult = Quiz.CurrentState;
                            }
                            else
                                goto again;
                        }
                        else
                        {
                            Console.WriteLine();    // In case transition.Execute() outputs something and then waits for a key
                            transitionResult = transition.Execute();
                            if (transitionResult == null)
                                break;
                            if (transitionResult.State != null)
                                Quiz.Transition(transitionResult.State);
                        }

                        if (transitionResult.JsMethod != null)
                            foreach (var socket in Sockets)
                                socket.SendMessage(new JsonDict { { "method", transitionResult.JsMethod }, { "params", transitionResult.JsParameters } });

                        break;
                }
                ClassifyJson.SerializeToFile(Quiz, file);
            }

            exit:
            return 0;
        }

        private static HttpResponse handle(HttpRequest req)
        {
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
            return HttpResponse.WebSocket(new QuizWebSocket());
        }
    }
}
