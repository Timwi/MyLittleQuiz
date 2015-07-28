using System;
using RT.Util.ExtensionMethods;
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
        private int? _selectedContestant;

        public Round1Elimination(string undoLine, QuestionBase[] questions, Contestant[] contestants, int? selectedContestant = null)
            : base(undoLine)
        {
            if (questions == null)
                throw new ArgumentNullException("questions");
            if (contestants == null)
                throw new ArgumentNullException("contestants");

            _questions = questions;
            for (int i = 0; i < contestants.Length; i++)
                contestants[i].Round1Number = i + 1;
            _contestants = contestants;
            _selectedContestant = selectedContestant;
        }

        private Round1Elimination() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Select(ConsoleKey.S, "Select contestant", _contestants, i => new Round1Elimination("Selected contestant #" + _contestants[i].Round1Number, _questions, _contestants, i));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\n{1}".Color(null).Fmt(
                    /* 0 */ "Contestants:",
                    /* 1 */ _contestants.Select((c, i) =>
                        (i == _selectedContestant ? "[".Color(ConsoleColor.Magenta, ConsoleColor.DarkRed) : null) +
                        "•".Color(ConsoleColor.White, ConsoleColor.DarkGreen).Repeat(c.Round1Correct)
                            .Concat("•".Color(ConsoleColor.Black, ConsoleColor.Red).Repeat(c.Round1Wrong))
                            .JoinColoredString() + c.Round1Number.ToString().Color(ConsoleColor.White) +
                        (i == _selectedContestant ? "]".Color(ConsoleColor.Magenta, ConsoleColor.DarkRed) : null))
                        .JoinColoredString(" ")
                );
            }
        }

        public override string JsMethod { get { return _selectedContestant == null ? "r1_showContestants" : "r1_select"; } }
        public override object JsParameters { get { return new { contestants = _contestants, selected = _selectedContestant }; } }
    }
}
