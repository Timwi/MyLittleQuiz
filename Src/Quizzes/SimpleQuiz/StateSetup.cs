using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util.Consoles;
using RT.Util.Serialization;
using RT.Util;
using RT.Util.Json;

namespace QuizGameEngine.Quizzes.SimpleQuiz
{
    public sealed class StateSetup : StateBase
    {
        public StateSetup(string undoLine, Tuple<string, string>[] questions, Contestant[] contestants = null)
            : base(undoLine, questions, contestants)
        {
        }

        private StateSetup() { }   // for Classify

        public override Transition[] Transitions
        {
            get
            {
                return Ut.NewArray(
                    Transition.String(ConsoleKey.N, "New contestant", "Contestant name: ", name => new StateSetup("Create contestant: {0}".Fmt(name), Questions, Contestants.Concat(name).ToArray()), true),
                    Transition.Select(ConsoleKey.D, "Delete contestant", Contestants, index => new StateSetup("Delete contestant: {0}".Fmt(Contestants[index]), Questions, Contestants.RemoveIndex(index))),
                    Transition.Simple(ConsoleKey.L, "List all questions (with answers!)", () =>
                    {
                        Console.WriteLine();
                        foreach (var q in Questions)
                        {
                            ConsoleUtil.WriteParagraphs(q.Item1.Color(ConsoleColor.Green));
                            ConsoleUtil.WriteParagraphs(("    " + q.Item2).Color(ConsoleColor.Red), 4);
                            Console.WriteLine();
                        }
                        Console.ReadKey();
                    }),
                    Transition.Simple(ConsoleKey.W, "Show welcome", "welcome"),
                    Transition.Simple(ConsoleKey.S, "Start game", () => new StateGame("Start game", Questions, Contestants).With("start", new { contestants = Contestants }))
                );
            }
        }
    }
}
