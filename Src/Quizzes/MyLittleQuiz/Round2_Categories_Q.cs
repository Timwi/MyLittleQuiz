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
        public Round2Data Data { get; private set; }

        public Round2_Categories_Q(Round2Data data) { Data = data; }
        private Round2_Categories_Q() { } // for Classify

        protected override string Round { get { return "r2"; } }

        public override QuestionBase CurrentQuestion { get { return Data.Categories[Data.SelectedCategory.Value].Questions[Data.SelectedQuestion.Value]; } }
        public override TransitionResult GiveAnswer(bool correct)
        {
            return new Round2_Categories_Q(Data.GiveAnswer(correct))
                .With("showA", new { answer = correct, round = Round }, jsJingle: (correct ? Jingle.Round1CorrectAnswer : Jingle.Round1WrongAnswer).ToString());
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0}\n\n{1}".Color(null).Fmt(
                    /* 0 */ Data.Describe,
                    /* 1 */ CurrentQuestion.Describe(Data.AnswerGiven)
                );
            }
        }

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                return Data.AnswerGiven == null
                    ? getAnswerTransitions()
                    : new[] { Transition.Simple(ConsoleKey.Spacebar, "Dismiss question", () => new Round2_Categories_ShowContestants(Data.DismissQuestion()).With(jsJingle: Jingle.Swoosh.ToString())) };
            }
        }

        public override string JsMethod { get { return Data.AnswerGiven == null ? "showQ" : "showQA"; } }
        public override string JsMusic { get { return Data.MusicStarted ? Music.Music2.ToString() : null; } }
        public override object JsParameters
        {
            get
            {
                return new
                {
                    question = CurrentQuestion,
                    answer = Data.AnswerGiven,
                    round = "r2"
                };
            }
        }
    }
}
