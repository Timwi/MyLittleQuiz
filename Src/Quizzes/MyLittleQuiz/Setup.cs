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
    public sealed class Setup : QuizStateBase, IClassifyObjectProcessor
    {
        public Setup()
        {
            Contestants = new List<Contestant>();
            DeletedContestants = new List<Contestant>();
            Questions = new List<QuestionBase>();
        }

        [ClassifyNotNull]
        public List<Contestant> Contestants { get; private set; }
        [ClassifyNotNull]
        public List<Contestant> DeletedContestants { get; private set; }
        [ClassifyNotNull]
        public List<QuestionBase> Questions { get; private set; }

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

                if (Contestants.Count > 0)
                    yield return Transition.Simple(ConsoleKey.S, "Start Round: Elimination Round", () => Round1Elimination.Create("Start game", Questions.ToArray(), Contestants.ToArray()));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White} contestants\n{1/White} questions"
                        .Color(null)
                        .Fmt(Contestants.Count, Questions.Count);
            }
        }

        public override string JsMethod { get { return null; } }
        public override object JsParameters { get { return null; } }

        void IClassifyObjectProcessor.BeforeSerialize() { }

        void IClassifyObjectProcessor.AfterDeserialize()
        {
            if (Questions.Count > 0)
                return;

            Questions.AddRange(Ut.NewArray<QuestionBase>(
                new SimpleQuestion { Difficulty = Difficulty.Easy, QuestionText = "What is the name of Rarity’s younger sister?", Answer = "Sweetie Belle" },
                new SimpleQuestion { Difficulty = Difficulty.Easy, QuestionText = "What is the name of Applejack’s younger sister?", Answer = "Apple Bloom" },
                new NOfQuestion { Difficulty = Difficulty.Medium, QuestionText = "Name two characters first introduced by name in <i>The Cutie Map</i> (S5 E01–02).", N = 2, Answers = new[] { "Starlight Glimmer", "Double Diamond", "Party Favor", "Sugar Belle" } }
            ));
        }
    }
}
