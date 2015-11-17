using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round4_Final_Q : MyLittleQuizStateBase
    {
        private Round4Data Data;

        public Round4_Final_Q(Round4Data data) { Data = data; }
        private Round4_Final_Q() { }    // for Classify

        public override QuestionBase CurrentQuestion { get { return Data.QuizData.Round4Questions[Data.QuestionIndex]; } }
        public override QuizStateBase GiveAnswer(object answer) { return new Round4_Final_Q(Data.GiveAnswer(answer)); }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{5}\n\n{0/White}\n{1/Cyan}\n\n{2/White}\n{3/Green}{4}".Color(null).Fmt(
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
                // If a contestant is out of the game, but the game is not over, say goodbye to them.
                var cOut = Data.ContestantsOutOfGame;
                if (cOut.Length > 0 && Data.Contestants.Length - cOut.Length > 1)
                {
                    yield return Transition.Simple(ConsoleKey.Spacebar, "Say goodbye to " + cOut.Select(ix => Data.Contestants[ix].Name).JoinString(separator: ", ", lastSeparator: " and "), () =>
                    {
                        var data = Data;
                        for (int i = cOut.Length - 1; i >= 0; i--)
                            data = data.RemoveContestant(cOut[i]);
                        return new Round4_Final_ShowContestants(data.DismissQuestion());
                    });
                    yield break;
                }

                // If only one contestant is left, the game is over.
                if (cOut.Length > 0 && Data.Contestants.Length - cOut.Length == 1)
                {
                    yield return Transition.Simple(ConsoleKey.Spacebar, "{0} wins. Congratulations!".Fmt(Data.Contestants.Where((c, i) => !cOut.Contains(i)).First().Name),
                        () => new Round4_Final_Congratulations(Data));
                    yield break;
                }

                // Otherwise, just continue with the game.
                yield return Transition.Simple(ConsoleKey.Spacebar, "Dismiss question", () => new Round4_Final_ShowContestants(Data.DismissQuestion()));
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
