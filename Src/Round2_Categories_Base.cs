using System;
using System.Linq;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy.MyLittleQuiz
{
    public abstract class Round2_Categories_Base : QuizStateBase
    {
        public Round2Data Data { get; private set; }

        public Round2_Categories_Base(Round2Data data) { Data = data; }
        protected Round2_Categories_Base() { } // for Classify

        protected Transition listContestantsTransition
        {
            get
            {
                return Transition.Simple(ConsoleKey.L, "List contestants in score order", () =>
                {
                    Console.WriteLine();
                    var sorted = Data.Contestants.OrderByDescending(c => c.Score).ToArray();
                    for (int i = 0; i < sorted.Length; i++)
                        ConsoleUtil.WriteLine((i < 6 ? "{0/Cyan} - {1/Yellow}" : "{0/DarkCyan} - {1/DarkYellow}").Color(null).Fmt(sorted[i].Score, sorted[i].Name));
                    ReadKey();
                });
            }
        }

        public override string JsMusic { get { return Data.MusicStarted ? Music.Music2.ToString() : null; } }
    }
}
