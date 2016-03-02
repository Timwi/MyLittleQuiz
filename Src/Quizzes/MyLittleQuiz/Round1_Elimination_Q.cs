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

        protected override string Round { get { return "r1"; } }

        public override QuestionBase CurrentQuestion { get { return Data.CurrentDifficulty.NullOr(cd => Data.CurrentQuestionIndex.NullOr(cqi => Data.Questions[cd][cqi])); } }
        public override QuizStateBase GiveAnswer(object answer) { return new Round1_Elimination_Q(Data.GiveAnswer(answer)); }

        public override ConsoleColoredString Describe { get { return Data.Describe; } }

        public override IEnumerable<Transition> Transitions { get { return Data.AnswerObject == null ? getAnswerTransitions() : transitionsAfterAnswer; } }

        private IEnumerable<Transition> transitionsAfterAnswer
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.Spacebar, "Dismiss question", () => Round1_Elimination.GetQuizState(Data.DismissQuestion()));
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
