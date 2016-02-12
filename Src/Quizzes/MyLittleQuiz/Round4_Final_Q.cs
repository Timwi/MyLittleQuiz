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

        public override QuestionBase CurrentQuestion { get { return Data.CurrentQuestion; } }
        public override QuizStateBase GiveAnswer(object answer) { return new Round4_Final_Q(Data.GiveAnswer(answer)); }

        public override ConsoleColoredString Describe
        {
            get
            {
                return Data.Describe;
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
