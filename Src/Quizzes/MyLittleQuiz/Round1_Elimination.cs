using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round1_Elimination : QuizStateBase
    {
        public Round1Data Data { get; private set; }

        public Round1_Elimination(Round1Data data) { Data = data; }
        private Round1_Elimination() { } // for Classify

        public static QuizStateBase GetQuizState(Round1Data data)
        {
            var through = data.Contestants.Where(c => c.IsThrough).ToArray();
            var throughAndRemaining = data.Contestants.Where(c => c.IsThrough || c.IsStillInGame).ToArray();
            var nextRoundContestants =
                through.Length == data.QuizData.Round1NumContestantsNeeded ? through :
                throughAndRemaining.Length == data.QuizData.Round1NumContestantsNeeded ? throughAndRemaining : null;

            if (nextRoundContestants != null)
                return new Round2_Categories_ShowContestants(new Round2Data(data.QuizData, (Round2Contestant[]) nextRoundContestants.Select(c => new Round2Contestant(c.Name, 0)).ToArray().Shuffle()), noScores: true);

            return new Round1_Elimination(data);
        }

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.SelectIndex(ConsoleKey.S, "Select contestant", Data.Contestants, index => new Round1_Elimination(Data.SelectContestant(index)));

                yield return Transition.Simple(ConsoleKey.R, "Select contestant at random", () =>
                {
                    var choosableContestants = Data.Contestants.SelectIndexWhere(c => c.IsStillInGame).ToArray();
                    var index = choosableContestants[Rnd.Next(choosableContestants.Length)];
                    return new Round1_Elimination(Data.SelectContestant(index));
                });

                if (Data.SelectedContestant != null)
                {
                    yield return Transition.Simple(ConsoleKey.Q, "Ask the question", () => new Round1_Elimination_Q(Data));
                    yield return Transition.Simple(ConsoleKey.D, "Disqualify", () => GetQuizState(Data.DisqualifySelectedContestant()));
                }
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return Data.Describe;
            }
        }

        public override string JsMethod { get { return Data.SelectedContestant == null ? "r1_showContestants" : "r1_select"; } }
        public override object JsParameters
        {
            get
            {
                return new
                {
                    contestants = Data.Contestants,
                    selected = Data.SelectedContestant
                };
            }
        }
    }
}
