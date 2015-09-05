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

        public StateGame(Tuple<string, string>[] questions, Contestant[] contestants, int? selContestant = null)
        {
            SelectedContestant = selContestant;
        }

        private StateGame() { }   // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.SelectIndex(ConsoleKey.S, "Select contestant", Contestants, i => new StateGame(Questions, Contestants, i).With("select", new { index = i }));

                if (SelectedContestant != null)
                {
                    yield return Transition.Simple(ConsoleKey.U, "Unselect contestant", () => new StateGame(Questions, Contestants, null).With("unselect"));
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
