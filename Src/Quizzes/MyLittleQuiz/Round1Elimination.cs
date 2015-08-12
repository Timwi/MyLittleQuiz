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
                yield return Transition.Select(ConsoleKey.S, "Select contestant", Contestants, i => new Round1Elimination("Selected contestant #" + (i + 1), Questions, Contestants, i));

                yield return Transition.Simple(ConsoleKey.R, "Select contestant at random", () =>
                {
                    var choosableContestants = Contestants.SelectIndexWhere(c => c.Round1Correct < 2 && c.Round1Wrong < 2).ToArray();
                    var index = choosableContestants[Rnd.Next(choosableContestants.Length)];
                    return new Round1Elimination("Selected contestant #{0} (random)".Fmt(index + 1), Questions, Contestants, index);
                });

                if (SelectedContestant != null)
                {
                    var difficulty = Contestants[SelectedContestant.Value].Round1Correct > 0 || !Questions.ContainsKey(Difficulty.Easy) || Questions[Difficulty.Easy].Length == 0 ? Difficulty.Medium : Difficulty.Easy;
                    yield return Transition.Simple(ConsoleKey.Q, "Ask question ({0})".Fmt(difficulty), () =>
                        new Round1EliminationQ("Asked {0} question".Fmt(difficulty), this, difficulty));
                }
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\n{1}{2}\n\n{3/White}\n{4}".Color(null).Fmt(
                    /* 0 */ "Contestants:",
                    /* 1 */ Contestants.Select((c, i) =>
                                    (i == SelectedContestant ? "[".Color(ConsoleColor.Magenta, ConsoleColor.DarkRed) : null) +
                                    "•".Color(ConsoleColor.White, ConsoleColor.DarkGreen).Repeat(c.Round1Correct).JoinColoredString() +
                                    "•".Color(ConsoleColor.Black, ConsoleColor.Red).Repeat(c.Round1Wrong).JoinColoredString() +
                                    (i + 1).ToString().Color(ConsoleColor.White, i == SelectedContestant ? ConsoleColor.DarkRed : (ConsoleColor?) null) +
                                    (i == SelectedContestant ? "]".Color(ConsoleColor.Magenta, ConsoleColor.DarkRed) : null))
                                    .JoinColoredString(" "),
                    /* 2 */ SelectedContestant == null ? null : "\n\n{0/White}\n{1/Green} {2/Red} {3/Yellow}".Color(null).Fmt("Selected contestant:", Contestants[SelectedContestant.Value].Round1Correct, Contestants[SelectedContestant.Value].Round1Wrong, Contestants[SelectedContestant.Value].Name),
                    /* 3 */ "Questions:",
                    /* 4 */ Questions.Select(kvp => "{0/Cyan}: {1/Magenta}".Color(null).Fmt(kvp.Key, kvp.Value.Length)).JoinColoredString("\n")
                );
            }
        }

        public override string JsMethod { get { return SelectedContestant == null ? "r1_showContestants" : "r1_select"; } }
        public override object JsParameters { get { return new { contestants = Contestants, selected = SelectedContestant }; } }
    }
}
