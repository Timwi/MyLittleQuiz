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
        public Setup()
        {
            Contestants = new List<Contestant>(); ;
            DeletedContestants = new List<Contestant>();
        }

        [ClassifyNotNull]
        public List<Contestant> Contestants { get; private set; }
        [ClassifyNotNull]
        public List<Contestant> DeletedContestants { get; private set; }

        [ClassifyNotNull]
        public QuizData Data = new QuizData();

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.String(ConsoleKey.A, "Add contestant", "Contestant name: ", "Roll number (r for random): ", (name, roll) =>
                {
                    Contestants.Add(new Contestant(name, roll == "r" ? Rnd.Next().ToString() : roll));
                    DeletedContestants.RemoveAll(c => c.Name == name && c.Roll == roll);
                });

                if (Contestants.Count > 0)
                    yield return Transition.Select(ConsoleKey.D, "Delete contestant", Contestants.ToArray<object>(), index =>
                    {
                        DeletedContestants.Add(Contestants[index]);
                        Contestants.RemoveAt(index);
                    });

                if (Contestants.Count > 0)
                    yield return Transition.Simple(ConsoleKey.L, "List contestants", () =>
                    {
                        foreach (var q in Contestants)
                            ConsoleUtil.WriteLine("{0/White} ({1/Green})".Color(null).Fmt(q.Name, q.Roll));
                        Program.ReadKey();
                    });

                if (DeletedContestants.Count > 0)
                    yield return Transition.Simple(ConsoleKey.K, "List deleted contestants", () =>
                    {
                        foreach (var q in DeletedContestants)
                            ConsoleUtil.WriteLine("{0/White} ({1/Green})".Color(null).Fmt(q.Name, q.Roll));
                        Program.ReadKey();
                    });

                if (DeletedContestants.Count > 0)
                    yield return Transition.Select(ConsoleKey.R, "Resurrect deleted contestant", DeletedContestants.ToArray<object>(), index =>
                    {
                        Contestants.Add(DeletedContestants[index]);
                        DeletedContestants.RemoveAt(index);
                    });

                yield return Transition.Simple(ConsoleKey.E, "Edit quiz data", () => Program.Edit(Data, new[] { "Quiz data" }));

                if (Contestants.Count > 0)
                    yield return Transition.Simple(ConsoleKey.S, "Start Round: Elimination Round", () => new Round1_Elimination("Start game", new Round1Data(Data, Contestants)));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White} contestants\n\n{1/White}\nNumber of contestants needed: {2/Cyan}".Color(null).Fmt(
                    /* 0 */ Contestants.Count,
                    /* 1 */ "ROUND 1 (Elimination)",
                    /* 2 */ Data.Round1NumContestantsNeeded
                );
            }
        }

        public override string JsMethod { get { return null; } }
        public override object JsParameters { get { return null; } }
    }
}
