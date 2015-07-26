using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round1Elimination : QuizStateBase
    {
        private QuestionBase[] _questions;
        private Contestant[] _contestants;
        private bool[] _hasCorrect;
        private bool[] _hasWrong;
        private int? _selectedContestant;

        public Round1Elimination(string undoLine, QuestionBase[] questions, Contestant[] contestants)
            : base(undoLine)
        {
            _questions = questions;
            _contestants = contestants;
            _hasCorrect = new bool[contestants.Length];
            _hasWrong = new bool[contestants.Length];
        }

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.Spacebar, "Show contestants", "showContestants", new
                {
                    contestants = _contestants,
                    correct = _hasCorrect,
                    wrong = _hasWrong,
                    selected = _selectedContestant
                });
            }
        }

        public override ConsoleColoredString Describe
        {
            get { throw new NotImplementedException(); }
        }

        public override string JsMethod
        {
            get { throw new NotImplementedException(); }
        }

        public override object JsParameters
        {
            get { throw new NotImplementedException(); }
        }
    }
}
