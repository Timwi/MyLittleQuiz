using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round1_Elimination_Q : MyLittleQuizStateBase
    {
        private Round1Data Data;

        public Round1_Elimination_Q(Round1Data data) { Data = data; }
        private Round1_Elimination_Q() { }    // for Classify

        public override QuestionBase CurrentQuestion { get { return Data.CurrentDifficulty.NullOr(cd => Data.CurrentQuestionIndex.NullOr(cqi => Data.Questions[cd][cqi])); } }
        public override QuizStateBase GiveAnswer(object answer) { return new Round1_Elimination_Q(Data.GiveAnswer(answer)); }

        public override ConsoleColoredString Describe { get { return Data.Describe; } }

        public override IEnumerable<Transition> Transitions { get { return Data.AnswerObject == null ? getAnswerTransitions() : transitionsAfterAnswer; } }

        private IEnumerable<Transition> transitionsAfterAnswer
        {
            get
            {
                var wouldBe = Data.DismissQuestion();

                var through = wouldBe.Contestants.Where(c => c.IsThrough).ToArray();
                var throughAndRemaining = wouldBe.Contestants.Where(c => c.IsThrough || c.IsStillInGame).ToArray();
                var nextRoundContestants =
                    through.Length == wouldBe.NumContestantsNeeded ? through :
                    throughAndRemaining.Length == wouldBe.NumContestantsNeeded ? throughAndRemaining : null;

                if (nextRoundContestants != null)
                    yield return Transition.Simple(ConsoleKey.Spacebar, "End of round congratulations", () => new Round2_Categories_ShowContestants(new Round2Data(Data.QuizData, (Round2Contestant[]) nextRoundContestants.Select(c => new Round2Contestant(c.Name, 0)).ToArray().Shuffle()), noScores: true));
                else
                    yield return Transition.Simple(ConsoleKey.Spacebar, "Dismiss question", () => new Round1_Elimination(wouldBe));
            }
        }

        public override string JsMethod { get { return Data.AnswerObject == null ? "showQ" : "showQA"; } }
        public override object JsParameters
        {
            get
            {
                return new
                {
                    question = CurrentQuestion,
                    answer = Data.AnswerObject,
                    round = "r1"
                };
            }
        }
    }
}
