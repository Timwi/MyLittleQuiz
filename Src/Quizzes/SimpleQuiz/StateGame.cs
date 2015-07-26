using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Json;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.SimpleQuiz
{
    public sealed class StateGame : StateBase
    {
        [ClassifyIgnoreIfDefault]
        public int? SelectedContestant { get; private set; }

        public StateGame(string undoLine, Tuple<string, string>[] questions, Contestant[] contestants, int? selContestant = null)
            : base(undoLine, questions, contestants)
        {
            SelectedContestant = selContestant;
        }

        private StateGame() { }   // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Select(ConsoleKey.S, "Select contestant", Contestants, i => new StateGame("Selected contestant: " + Contestants[i], Questions, Contestants, i).With("select", new { index = i }));

                if (SelectedContestant != null)
                {
                    yield return Transition.Simple(ConsoleKey.U, "Unselect contestant", () => new StateGame("Unselected contestant: " + Contestants[SelectedContestant.Value], Questions, Contestants, null).With("unselect"));
                    yield return Transition.Simple(ConsoleKey.A, "Ask a question", () => Rnd.Next(Questions.Length).Apply(qi => new StateQuestion(this, qi)));
                }
            }
        }

        public override ConsoleColoredString Describe { get { return DescribeSel(SelectedContestant); } }

        public override string JsMethod { get { return "showContestants"; } }
        public override object JsParameters
        {
            get
            {
                return new
                {
                    contestants = Contestants,
                    selected = SelectedContestant
                };
            }
        }
    }
}
