using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Text;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round2_Categories_ShowContestants : QuizStateBase
    {
        public bool NoScores { get; private set; }
        public Round2Data Data { get; private set; }

        public Round2_Categories_ShowContestants(Round2Data data, bool noScores = false)
        {
            NoScores = noScores;
            Data = data;
        }
        private Round2_Categories_ShowContestants() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.I, "Intro", () =>
                {
                    // TODO
                });

                if (NoScores)
                    yield return Transition.Simple(ConsoleKey.S, "Show scores", () => new Round2_Categories_ShowContestants(Data));
                else
                    yield return Transition.Simple(ConsoleKey.C, "Show categories", () => new Round2_Categories_ShowCategories(Data));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                var tt = new TextTable { ColumnSpacing = 2 };
                for (int i = 0; i < Data.Contestants.Length; i++)
                {
                    tt.SetCell(0, i, (i + 1).ToString().Color(ConsoleColor.Yellow));
                    tt.SetCell(1, i, Data.Contestants[i].Name.Color(ConsoleColor.Green));
                }
                return tt.ToColoredString();
            }
        }

        public override string JsMethod
        {
            get { return "r2_showContestants"; }
        }

        public override object JsParameters
        {
            get
            {
                return new
                {
                    contestants = Data.Contestants,
                    current = Data.CurrentContestant,
                    noscores = NoScores
                };
            }
        }
    }
}
