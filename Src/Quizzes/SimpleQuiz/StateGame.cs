using System;
using System.Linq;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Json;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.SimpleQuiz
{
    public sealed class StateGame : StateBase, IHasJsTransition
    {
        [ClassifyIgnoreIfDefault]
        public int? SelectedContestant { get; private set; }

        public StateGame(string undoLine, Tuple<string, string>[] questions, Contestant[] contestants, int? selContestant = null)
            : base(undoLine, questions, contestants)
        {
            SelectedContestant = selContestant;
        }

        private StateGame() { }   // for Classify

        public override Transition[] Transitions
        {
            get
            {
                return Ut.NewArray<Transition>(
                    Transition.Select(ConsoleKey.S, "Select contestant", Contestants, i => new StateGame("Selected contestant: " + Contestants[i], Questions, Contestants, i).With("select", new { index = i })),
                    SelectedContestant.NullOr(i => Transition.Simple(ConsoleKey.U, "Unselect contestant", () => new StateGame("Unselected contestant: " + Contestants[i], Questions, Contestants, null).With("unselect"))),
                    SelectedContestant.NullOr(i => Transition.Simple(ConsoleKey.A, "Ask a question", () => Rnd.Next(Questions.Length).Apply(qi => new StateQuestion(this, qi).With("showQuestion", new { question = Questions[qi].Item1 }))))
                )
                    .Where(t => t != null)
                    .ToArray();
            }
        }

        public override ConsoleColoredString Describe { get { return DescribeSel(SelectedContestant); } }

        public string JsMethod { get { return "start"; } }
        public object JsParameters { get { return new { contestants = Contestants }; } }
    }
}
