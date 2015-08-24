using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round1_Elimination_Q : QuizStateBase
    {
        public Round1Data Data { get; private set; }

        public Round1_Elimination_Q(Round1Data data) { Data = data; }
        private Round1_Elimination_Q() { }    // for Classify

        public QuestionBase CurrentQuestion { get { return Data.Questions[Data.CurrentDifficulty.Value][Data.QuestionIndex[Data.CurrentDifficulty.Value]]; } }

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

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                // Contestant has not yet answered the question
                if (Data.AnswerObject == null)
                    return CurrentQuestion.CorrectAnswerInfos.Concat(Tuple.Create(ConsoleKey.Z, "Wrong", (object) false)).Select(answerInfo => Transition.Simple(
                        answerInfo.Item1, "Answer: " + answerInfo.Item2, () => new Round1_Elimination_Q(Data.GiveAnswer(answerInfo.Item3)).With("r1_showA", new { answer = answerInfo.Item3 })));

                // Contestant HAS answered the question
                var wouldBe = Data.DismissQuestion();

                var through = wouldBe.Contestants.Where(c => c.IsThrough).ToArray();
                var throughAndRemaining = wouldBe.Contestants.Where(c => c.IsThrough || c.IsStillInGame).ToArray();
                var nextRoundContestants =
                    through.Length == wouldBe.NumContestantsNeeded ? through :
                    throughAndRemaining.Length == wouldBe.NumContestantsNeeded ? throughAndRemaining : null;

                if (nextRoundContestants != null)
                    return new[] { Transition.Simple(ConsoleKey.Spacebar, "End of round congratulations", () => new Round2_Categories_ShowContestants(new Round2Data(Data.QuizData, nextRoundContestants.Select(c => new Round2Contestant(c.Name, 0)).ToArray()))) };
                else
                    return new[] { Transition.Simple(ConsoleKey.Spacebar, "Dismiss question", () => new Round1_Elimination(wouldBe)) };
            }
        }

        public override string JsMethod { get { return Data.AnswerObject == null ? "r1_showQ" : "r1_showQA"; } }
        public override object JsParameters
        {
            get
            {
                return new
                {
                    question = Data.Questions[Data.CurrentDifficulty.Value][Data.QuestionIndex[Data.CurrentDifficulty.Value]],
                    answer = Data.AnswerObject
                };
            }
        }
    }
}
