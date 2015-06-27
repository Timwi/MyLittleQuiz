using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using RT.Servers;
using RT.TagSoup;
using RT.Util;
using RT.Util.CommandLine;
using RT.Util.Consoles;
using RT.Util.Serialization;

namespace QuizGameEngine
{
    static class Program
    {
        public static QuizStateBase QuizState;
        public static Stack<QuizStateBase> Redo = new Stack<QuizStateBase>();
        public static HttpServer Server;

        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length == 2 && args[0] == "--post-build-check")
                return Ut.RunPostBuildChecks(args[1], Assembly.GetExecutingAssembly());

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
                ClassifyJson.SerializeToFile(start.Cmd.StartState, start.OutputFile);
                ConsoleUtil.WriteLine("File saved.");
                return 0;
            }

            var file = ((QuizCmdLoad) cmd.QuizCmd).File;
            QuizState = ClassifyJson.DeserializeFile<QuizStateBase>(file);

            Server = new HttpServer(24567);
            Server.Handler = handle;
            Server.StartListening();

            while (true)
            {
                ConsoleUtil.WriteLine(new string('═', Console.BufferWidth - 1).Color(ConsoleColor.White));
                ConsoleUtil.WriteLine(QuizState.Describe);
                foreach (var t in QuizState.Transitions)
                    ConsoleUtil.WriteLine("{0/White}: {1/Cyan}".Color(null).Fmt(t.Key, t.Name));
                again:
                var keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        goto exit;

                    case ConsoleKey.Backspace:
                        if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift))
                        {
                            if (Redo.Count == 0)
                                goto again;
                            QuizState = Redo.Pop();
                        }
                        else
                        {
                            if (QuizState.PreviousState == null)
                                goto again;
                            Redo.Push(QuizState);
                            QuizState = QuizState.PreviousState;
                        }
                        break;

                    default:
                        var transition = QuizState.Transitions.FirstOrDefault(q => q.Key == keyInfo.Key);
                        if (transition == null)
                            goto again;
                        var state = transition.Execute();
                        if (state == null)
                            continue;
                        Redo = new Stack<QuizStateBase>();
                        QuizState = state;
                        break;
                }
                ClassifyJson.SerializeToFile(QuizState, file);
            }

            exit:
            return 0;
        }

        private static HttpResponse handle(HttpRequest req)
        {
            if (req.Url.Path == "/socket")
                return new HttpResponseWebSocket(new QuizWebSocket());

            return HttpResponse.Html(
                new HTML(
                    new HEAD(
                        new TITLE("Quiz"),
                        new SCRIPTLiteral(@""),
                        new META { httpEquiv = "Content-type", content = "text/html; charset=utf-8" }),
                    new BODY(
                        "Quizzie!")));
        }
    }
}
