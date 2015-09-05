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

        public override QuestionBase CurrentQuestion { get { return Data.Questions[Data.CurrentDifficulty.Value][Data.QuestionIndex[Data.CurrentDifficulty.Value]]; } }
        public override QuizStateBase GiveAnswer(object answer) { return new Round1_Elimination_Q(Data.GiveAnswer(answer)); }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\n{1/Cyan}\n\n{2/White}\n{3/Green}{4}\n\n{5}".Color(null).Fmt(
                    /* 0 */ "Question:",
                    /* 1 */ CurrentQuestion.QuestionFullText.WordWrap(ConsoleUtil.WrapToWidth(), 4).JoinColoredString(Environment.NewLine),
                    /* 2 */ "Answer(s):",
                    /* 3 */ CurrentQuestion.AnswerFullText.WordWrap(ConsoleUtil.WrapToWidth(), 4).JoinColoredString(Environment.NewLine),
                    /* 4 */ Data.AnswerObject == null ? null : "\n\nAnswer given".Color(Data.AnswerObject.Equals(false) ? ConsoleColor.Red : ConsoleColor.Green),
                    /* 5 */ Data.Describe
                );
            }
        }

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
                    yield return Transition.Simple(ConsoleKey.Spacebar, "End of round congratulations", () => new Round2_Categories_ShowContestants(new Round2Data(Data.QuizData, nextRoundContestants.Select(c => new Round2Contestant(c.Name, 0)).ToArray()), noScores: true));
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
