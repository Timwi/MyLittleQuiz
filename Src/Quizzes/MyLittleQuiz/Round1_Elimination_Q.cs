using System;
using System.Collections.Generic;
using RT.Util;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round1_Elimination_Q : MyLittleQuizStateBase
    {
        private Round1Data Data;

        public Round1_Elimination_Q(Round1Data data) { Data = data; }
        private Round1_Elimination_Q() { }    // for Classify

        protected override string Round { get { return "r1"; } }

        public override QuestionBase CurrentQuestion { get { return Data.CurrentDifficulty.NullOr(cd => Data.CurrentQuestionIndex.NullOr(cqi => Data.Questions[cd][cqi])); } }
        public override TransitionResult GiveAnswer(bool correct)
        {
            return new Round1_Elimination_Q(Data.GiveAnswer(correct))
                .With("showA", new { answer = correct, round = Round }, jsJingle: (correct ? Jingle.Round1CorrectAnswer : Jingle.Round1WrongAnswer).ToString());
        }

        public override ConsoleColoredString Describe { get { return Data.Describe; } }

        public override IEnumerable<Transition> Transitions { get { return Data.AnswerGiven == null ? getAnswerTransitions() : transitionsAfterAnswer; } }

        private IEnumerable<Transition> transitionsAfterAnswer
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.Spacebar, "Dismiss question", () => Round1_Elimination.TransitionTo(Data.DismissQuestion()));
            }
        }

        public override string JsMethod { get { return Data.AnswerGiven == null ? "showQ" : "showQA"; } }
        public override string JsMusic { get { return Data.MusicStarted ? Music.Music1.ToString() : null; } }
        public override object JsParameters
        {
            get
            {
                return new
                {
                    question = CurrentQuestion,
                    answer = Data.AnswerGiven,
                    round = "r1"
                };
            }
        }
    }
}
