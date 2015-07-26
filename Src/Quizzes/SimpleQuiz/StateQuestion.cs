using System;
using System.Collections.Generic;
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

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.G, "Correct", () => new StateQuestionAnswered(this, true).With("correct"));
                yield return Transition.Simple(ConsoleKey.M, "Wrong", () => new StateQuestionAnswered(this, false).With("wrong"));
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

        public override string JsMethod { get { return "showQuestion"; } }
        public override object JsParameters
        {
            get
            {
                return new
                {
                    question = Game.Questions[QuestionIndex].Item1,
                    answer = Game.Questions[QuestionIndex].Item2
                };
            }
        }
    }
}
