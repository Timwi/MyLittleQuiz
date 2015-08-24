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
        public StateSetup(Tuple<string, string>[] questions, Contestant[] contestants = null)
            : base(questions, contestants)
        {
        }

        private StateSetup() { }   // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.String(ConsoleKey.N, "New contestant", "Contestant name: ", name => new StateSetup(Questions, Contestants.Concat(name).ToArray()), true);
                yield return Transition.Select(ConsoleKey.D, "Delete contestant", Contestants, index => new StateSetup(Questions, Contestants.RemoveIndex(index)));
                yield return Transition.Simple(ConsoleKey.L, "List all questions (with answers!)", () =>
                {
                    Console.WriteLine();
                    foreach (var q in Questions)
                    {
                        ConsoleUtil.WriteParagraphs(q.Item1.Color(ConsoleColor.Green));
                        ConsoleUtil.WriteParagraphs(("    " + q.Item2).Color(ConsoleColor.Red));
                        Console.WriteLine();
                    }
                    Program.ReadKey();
                });
                yield return Transition.Simple(ConsoleKey.W, "Show welcome", "welcome");
                yield return Transition.Simple(ConsoleKey.S, "Start game", () => new StateGame(Questions, Contestants));
            }
        }

        public override string JsMethod { get { return null; } }
        public override object JsParameters { get { return null; } }
    }
}
