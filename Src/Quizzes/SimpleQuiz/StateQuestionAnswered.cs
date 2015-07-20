using System;
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
            : base(correct ? "Correct answer" : "Wrong answer")
        {
            QuestionState = questionState;
            Correct = correct;
        }

        private StateQuestionAnswered() { }    // for Classify

        public override Transition[] Transitions
        {
            get
            {
                return Ut.NewArray(
                    Transition.Simple(ConsoleKey.C, "Continue", () =>
                        (Correct
                            ? QuestionState.Game.Contestants.ReplaceIndex(QuestionState.Game.SelectedContestant.Value, QuestionState.Game.Contestants[QuestionState.Game.SelectedContestant.Value].IncScore())
                                .Apply(newContestants =>
                                    new StateGame("Go back to game", QuestionState.Game.Questions.RemoveIndex(QuestionState.QuestionIndex), newContestants))
                            : QuestionState.Game).With())
                );
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return (Correct ? "Correct" : "Wrong").Color(ConsoleColor.White);
            }
        }
    }
}
