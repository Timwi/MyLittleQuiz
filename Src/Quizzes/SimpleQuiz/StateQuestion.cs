using System;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.SimpleQuiz
{
    public class StateQuestion : QuizStateBase
    {
        public StateGame Game { get; private set; }
        public int QuestionIndex { get; private set; }

        public StateQuestion(StateGame game, int questionIndex)
            : base("Asked question: " + game.Questions[questionIndex].Item1)
        {
            Game = game;
            QuestionIndex = questionIndex;
        }

        private StateQuestion() { }    // for Classify

        public override Transition[] Transitions
        {
            get
            {
                return Ut.NewArray(
                    Transition.Simple(ConsoleKey.G, "Correct", () => new StateQuestionAnswered(this, true).With("correct")),
                    Transition.Simple(ConsoleKey.M, "Wrong", () => new StateQuestionAnswered(this, false).With("wrong"))
                );
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\n{1/Green}\n\n{2/White}\n{3/Red}".Color(null).Fmt(
                    "Question:",
                    Game.Questions[QuestionIndex].Item1,
                    "Answer:",
                    Game.Questions[QuestionIndex].Item2
                );
            }
        }
    }
}
