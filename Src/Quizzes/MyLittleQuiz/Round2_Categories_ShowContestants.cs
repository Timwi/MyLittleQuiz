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
    public sealed class Round2_Categories_ShowContestants : Round2_Categories_Base
    {
        public bool NoScores { get; private set; }

        public Round2_Categories_ShowContestants(Round2Data data, bool noScores = false)
            : base(data)
        {
            NoScores = noScores;
        }
        private Round2_Categories_ShowContestants() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                if (NoScores)
                {
                    yield return Transition.Simple(ConsoleKey.I, "Intro", () =>
                    {
                        // TODO
                    });
                    yield return Transition.Simple(ConsoleKey.S, "Show scores", () => new Round2_Categories_ShowContestants(Data));
                }
                else
                    yield return Transition.Simple(ConsoleKey.C, "Show categories", () => new Round2_Categories_ShowCategories(Data));

                yield return listContestantsTransition;

                yield return Transition.Simple(ConsoleKey.N, "Go to next round", () =>
                    new Round3_SetPoker_MakeTeams(Data.QuizData, Data.Contestants.OrderByDescending(c => c.Score).Take(Data.NumContestantsNeeded).Select(c => c.Name).ToArray()));
            }
        }

        public override ConsoleColoredString Describe { get { return Data.Describe; } }
        public override string JsMethod { get { return "r2_showContestants"; } }
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
