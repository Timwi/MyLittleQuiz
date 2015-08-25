using System;
using RT.Util;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    class Round2_Categories_Q : QuizStateBase
    {
        private Round2Data Data;

        public Round2_Categories_Q(Round2Data data) { Data = data; }
        private Round2_Categories_Q() { }    // for Classify

        public QuestionBase CurrentQuestion { get { return Data.Categories[Data.SelectedCategory.Value].Questions[Data.SelectedQuestion.Value]; } }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0}\n\n{1}".Color(null).Fmt(
                    /* 0 */ CurrentQuestion.Describe(Data.AnswerObject),
                    /* 1 */ Data.Describe
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
                        answerInfo.Item1, "Answer: " + answerInfo.Item2, () => new Round2_Categories_Q(Data.GiveAnswer(answerInfo.Item3)).With("r2_showA", new { answer = answerInfo.Item3 })));

                // Contestant HAS answered the question
                return new[] { Transition.Simple(ConsoleKey.Spacebar, "Dismiss question", () => new Round2_Categories_ShowCategories(Data.DismissQuestion())) };
            }
        }

        public override string JsMethod { get { return Data.AnswerObject == null ? "r2_showQ" : "r2_showQA"; } }
        public override object JsParameters
        {
            get
            {
                return new
                {
                    question = CurrentQuestion,
                    answer = Data.AnswerObject
                };
            }
        }
    }
}
