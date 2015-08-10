using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util.Consoles;
using RT.Util.Serialization;
using RT.Util;
using RT.Util.Json;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Setup : QuizStateBase
    {
        public Setup() { }

        [ClassifyNotNull]
        public List<Contestant> _contestants = new List<Contestant>();
        [ClassifyNotNull]
        private List<Contestant> _deletedContestants = new List<Contestant>();
        [ClassifyNotNull]
        private List<QuestionBase> _questions = new List<QuestionBase>();

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.String(ConsoleKey.A, "Add contestant", "Contestant name: ", "Roll number (r for random): ", (name, roll) =>
                {
                    _contestants.Add(new Contestant(name, roll == "r" ? Rnd.Next().ToString() : roll));
                    _deletedContestants.RemoveAll(c => c.Name == name && c.Roll == roll);
                });

                if (_contestants.Count > 0)
                    yield return Transition.Select(ConsoleKey.D, "Delete contestant", _contestants.ToArray<object>(), index =>
                    {
                        _deletedContestants.Add(_contestants[index]);
                        _contestants.RemoveAt(index);
                    });

                if (_contestants.Count > 0)
                    yield return Transition.Simple(ConsoleKey.L, "List contestants", () =>
                    {
                        foreach (var q in _contestants)
                            ConsoleUtil.WriteLine("{0/White} ({1/Green})".Color(null).Fmt(q.Name, q.Roll));
                        Program.ReadKey();
                    });

                if (_deletedContestants.Count > 0)
                    yield return Transition.Simple(ConsoleKey.K, "List deleted contestants", () =>
                    {
                        foreach (var q in _deletedContestants)
                            ConsoleUtil.WriteLine("{0/White} ({1/Green})".Color(null).Fmt(q.Name, q.Roll));
                        Program.ReadKey();
                    });

                if (_deletedContestants.Count > 0)
                    yield return Transition.Select(ConsoleKey.R, "Resurrect deleted contestant", _deletedContestants.ToArray<object>(), index =>
                    {
                        _contestants.Add(_deletedContestants[index]);
                        _deletedContestants.RemoveAt(index);
                    });

                if (_contestants.Count > 0)
                    yield return Transition.Simple(ConsoleKey.S, "Start Round: Elimination Round", () => Round1Elimination.Create("Start game", _questions.ToArray(), _contestants.ToArray()));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return @"
{0/White} contestants
"
                        .Color(null)
                        .Fmt(_contestants.Count);
            }
        }

        public override string JsMethod { get { return null; } }
        public override object JsParameters { get { return null; } }
    }
}
