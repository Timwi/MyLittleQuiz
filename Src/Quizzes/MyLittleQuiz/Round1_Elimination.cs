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
    public sealed class Round1_Elimination : QuizStateBase
    {
        public Round1Data Data { get; private set; }

        public Round1_Elimination(string undoLine, Round1Data data) : base(undoLine) { Data = data; }
        private Round1_Elimination() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Select(ConsoleKey.S, "Select contestant", Data.Contestants, index => new Round1_Elimination("Selected contestant #" + (index + 1), Data.SelectContestant(index)));

                yield return Transition.Simple(ConsoleKey.R, "Select contestant at random", () =>
                {
                    var choosableContestants = Data.Contestants.SelectIndexWhere(c => c.IsStillInGame).ToArray();
                    var index = choosableContestants[Rnd.Next(choosableContestants.Length)];
                    return new Round1_Elimination("Selected contestant #{0} (random)".Fmt(index + 1), Data.SelectContestant(index));
                });

                if (Data.SelectedContestant != null)
                {
                    var difficulty = Data.Contestants[Data.SelectedContestant.Value].NumCorrect > 0 || !Data.Questions.ContainsKey(Difficulty.Easy) || Data.Questions[Difficulty.Easy].Length == 0 ? Difficulty.Medium : Difficulty.Easy;
                    yield return Transition.Simple(ConsoleKey.Q, "Ask question ({0})".Fmt(difficulty), () => new Round1_Elimination_Q("Asked {0} question".Fmt(difficulty), Data.AskQuestion(difficulty)));
                }
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return Data.Describe;
            }
        }

        public override string JsMethod { get { return Data.SelectedContestant == null ? "r1_showContestants" : "r1_select"; } }
        public override object JsParameters { get { return new { contestants = Data.Contestants, selected = Data.SelectedContestant }; } }
    }
}
