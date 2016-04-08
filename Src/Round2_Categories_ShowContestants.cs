using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy.MyLittleQuiz
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
                    yield return Transition.Simple(ConsoleKey.I, "Intro Round 2", "r2_intro", jsJingle: Jingle.Round2Start.ToString());
                    yield return Transition.Simple(ConsoleKey.S, "Show scores", () => new Round2_Categories_ShowContestants(Data).With(jsJingle: Jingle.Swoosh.ToString()));
                }
                else
                    yield return Transition.Simple(ConsoleKey.C, "Show categories", () => new Round2_Categories_ShowCategories(Data).With(jsJingle: Jingle.Swoosh.ToString()));

                yield return listContestantsTransition;

                yield return Transition.Simple(ConsoleKey.N, "Go to next round", () =>
                    new Round3_SetPoker_MakeTeams(Data.QuizData,
                        Data.Contestants.OrderByDescending(c => c.Score).Take(Data.NumContestantsNeeded).Select(c => new ContestantAndScore(c.Name, c.Score)).ToArray()).With(jsJingle: Jingle.Tada.ToString()));
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
                    contestants = Data.Contestants.OrderByDescending(c => c.Score).ToArray(),
                    noscores = NoScores
                };
            }
        }
    }
}
