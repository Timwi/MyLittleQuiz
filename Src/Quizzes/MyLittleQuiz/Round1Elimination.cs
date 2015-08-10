using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round1Elimination : QuizStateBase
    {
        public Dictionary<Difficulty, QuestionBase[]> Questions { get; private set; }
        public Contestant[] Contestants { get; private set; }
        public int? SelectedContestant { get; private set; }

        public static Round1Elimination Create(string undoLine, QuestionBase[] questions, Contestant[] contestants)
        {
            if (questions == null)
                throw new ArgumentNullException("questions");
            if (contestants == null)
                throw new ArgumentNullException("contestants");

            for (int i = 0; i < contestants.Length; i++)
                contestants[i].Round1Number = i + 1;

            return new Round1Elimination(undoLine, questions.GroupBy(q => q.Difficulty).ToDictionary(g => g.Key, g => g.ToList().Shuffle().ToArray()), contestants);
        }

        public Round1Elimination(string undoLine, Dictionary<Difficulty, QuestionBase[]> questions, Contestant[] contestants, int? selectedContestant = null)
            : base(undoLine)
        {
            if (questions == null)
                throw new ArgumentNullException("questions");
            if (contestants == null)
                throw new ArgumentNullException("contestants");

            Questions = questions;
            Contestants = contestants;
            SelectedContestant = selectedContestant;
        }

        private Round1Elimination() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Select(ConsoleKey.S, "Select contestant", Contestants, i => new Round1Elimination("Selected contestant #" + Contestants[i].Round1Number, Questions, Contestants, i));
                if (SelectedContestant != null)
                {
                    var dfty = Contestants[SelectedContestant.Value].Round1Correct > 0 ? Difficulty.Medium : Difficulty.Easy;
                    yield return Transition.Simple(ConsoleKey.Q, "Ask question ({0})".Fmt(dfty), () =>
                        new Round1EliminationQ("Asked {0} question".Fmt(dfty), this, dfty));
                }
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\n{1}".Color(null).Fmt(
                    /* 0 */ "Contestants:",
                    /* 1 */ Contestants.Select((c, i) =>
                        (i == SelectedContestant ? "[".Color(ConsoleColor.Magenta, ConsoleColor.DarkRed) : null) +
                        "•".Color(ConsoleColor.White, ConsoleColor.DarkGreen).Repeat(c.Round1Correct)
                            .Concat("•".Color(ConsoleColor.Black, ConsoleColor.Red).Repeat(c.Round1Wrong))
                            .JoinColoredString() +
                        c.Round1Number.ToString().Color(ConsoleColor.White, i == SelectedContestant ? ConsoleColor.DarkRed : (ConsoleColor?) null) +
                        (i == SelectedContestant ? "]".Color(ConsoleColor.Magenta, ConsoleColor.DarkRed) : null))
                        .JoinColoredString(" ")
                );
            }
        }

        public override string JsMethod { get { return SelectedContestant == null ? "r1_showContestants" : "r1_select"; } }
        public override object JsParameters { get { return new { contestants = Contestants, selected = SelectedContestant }; } }
    }
}
