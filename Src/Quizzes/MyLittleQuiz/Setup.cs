using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

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
                    yield return Transition.SelectIndex(ConsoleKey.D, "Delete contestant", Contestants.ToArray(), index =>
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
                    yield return Transition.SelectIndex(ConsoleKey.R, "Resurrect deleted contestant", DeletedContestants.ToArray(), index =>
                    {
                        Contestants.Add(DeletedContestants[index]);
                        DeletedContestants.RemoveAt(index);
                    });

                if (Contestants.Count > 0)
                    yield return Transition.Simple(ConsoleKey.S, "Start Round: Elimination Round", () => new Round1_Elimination(new Round1Data(Data, Contestants)));
                if (Contestants.Count == 6)
                    yield return Transition.Simple(ConsoleKey.T, "DEBUG: Start Round 3: Set Poker", () => new Round3_SetPoker_MakeTeams(Data, Contestants.Select(c => c.Name).ToArray()));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White} contestants".Color(null).Fmt(Contestants.Count);
            }
        }

        public override string JsMethod { get { return null; } }
        public override object JsParameters { get { return null; } }
    }
}
