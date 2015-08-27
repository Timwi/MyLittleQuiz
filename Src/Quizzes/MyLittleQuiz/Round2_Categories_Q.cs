using System;
using RT.Util;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    class Round2_Categories_Q : MyLittleQuizStateBase
    {
        private Round2Data Data;

        public Round2_Categories_Q(Round2Data data) { Data = data; }
        private Round2_Categories_Q() { }    // for Classify

        public override QuestionBase CurrentQuestion { get { return Data.Categories[Data.SelectedCategory.Value].Questions[Data.SelectedQuestion.Value]; } }
        public override QuizStateBase GiveAnswer(object answer) { return new Round2_Categories_Q(Data.GiveAnswer(answer)); }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0}\n\n{1}".Color(null).Fmt(
                    /* 0 */ CurrentQuestion.Describe(Data.AnswerObject),
                    /* 1 */ Data.DescribeContestants
                );
            }
        }

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                return Data.AnswerObject == null
                    ? getAnswerTransitions()
                    : new[] { Transition.Simple(ConsoleKey.Spacebar, "Dismiss question", () => new Round2_Categories_ShowCategories(Data.DismissQuestion())) };
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
                    round = "r2"
                };
            }
        }
    }
}
