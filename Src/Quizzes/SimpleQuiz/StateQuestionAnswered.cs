using System;
using System.Collections.Generic;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.SimpleQuiz
{
    public sealed class StateQuestionAnswered : QuizStateBase
    {
        public StateQuestion QuestionState { get; private set; }
        public bool Correct { get; private set; }

        public StateQuestionAnswered(StateQuestion questionState, bool correct)
        {
            QuestionState = questionState;
            Correct = correct;
        }

        private StateQuestionAnswered() { }    // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.C, "Continue", () =>
                    Correct
                        ? QuestionState.Game.Contestants.ReplaceIndex(QuestionState.Game.SelectedContestant.Value, QuestionState.Game.Contestants[QuestionState.Game.SelectedContestant.Value].IncScore())
                            .Apply(newContestants =>
                                new StateGame(QuestionState.Game.Questions.RemoveIndex(QuestionState.QuestionIndex), newContestants))
                        : QuestionState.Game);
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return (Correct ? "Correct" : "Wrong").Color(ConsoleColor.White);
            }
        }

        public override string JsMethod { get { return "showQuestion"; } }
        public override object JsParameters
        {
            get
            {
                return new
                {
                    question = QuestionState.Game.Questions[QuestionState.QuestionIndex].Item1,
                    answer = QuestionState.Game.Questions[QuestionState.QuestionIndex].Item2,
                    correct = Correct
                };
            }
        }
    }
}
