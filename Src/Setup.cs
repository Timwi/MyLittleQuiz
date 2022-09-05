using System;
using System.Collections.Generic;
using RT.Serialization;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace Trophy.MyLittleQuiz
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

        public Music? CurrentMusic { get; private set; } = null;
        public Jingle? CurrentJingle { get; private set; } = null;

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
                        ReadKey();
                    });

                if (DeletedContestants.Count > 0)
                    yield return Transition.Simple(ConsoleKey.K, "List deleted contestants", () =>
                    {
                        foreach (var q in DeletedContestants)
                            ConsoleUtil.WriteLine("{0/White} ({1/Green})".Color(null).Fmt(q.Name, q.Roll));
                        ReadKey();
                    });

                if (DeletedContestants.Count > 0)
                    yield return Transition.SelectIndex(ConsoleKey.R, "Resurrect deleted contestant", DeletedContestants.ToArray(), index =>
                    {
                        Contestants.Add(DeletedContestants[index]);
                        DeletedContestants.RemoveAt(index);
                    });

                if (Contestants.Count > 0)
                    yield return Transition.Simple(ConsoleKey.S, "Start", () => new Round1_Elimination_Beginning(new Round1Data(Data, Contestants.ToArray().Shuffle())));

                if (CurrentMusic == null)
                    yield return Transition.Select(ConsoleKey.M, "Set music", EnumStrong.GetValues<Music>(), m => m.ToString(), m => this.ApplyToClone(th => { th.CurrentMusic = m; th.CurrentJingle = null; }));
                else
                    yield return Transition.Simple(ConsoleKey.M, "Mute music", () => this.ApplyToClone(th => { th.CurrentMusic = null; th.CurrentJingle = null; }));

                yield return Transition.Select(ConsoleKey.J, "Play jingle", EnumStrong.GetValues<Jingle>(), j => j.ToString(), j => this.ApplyToClone(th => { th.CurrentJingle = j; }));

                yield return Transition.Simple(ConsoleKey.I, "Play intro", "intro");

                yield return Transition.Select(ConsoleKey.T, "Test intros etc.",
                    Ut.NewArray(
                        new { Method = "r1_intro", Params = (object) null, Jingle = (Jingle?) Jingle.Round1Start },
                        new { Method = "r2_intro", Params = (object) null, Jingle = (Jingle?) Jingle.Round2Start },
                        new { Method = "r3_intro", Params = (object) null, Jingle = (Jingle?) Jingle.Round3Start },
                        new { Method = "r4_intro", Params = (object) null, Jingle = (Jingle?) Jingle.Round4Start },
                        new { Method = "blank", Params = (object) new { bgclass = "r1" }, Jingle = (Jingle?) null }
                    ),
                    //inf => "{0/Yellow} · {1/Cyan}".Color(null).Fmt(inf.Method, inf.Jingle),
                    inf => inf.ToString().Color(ConsoleColor.Yellow),
                    inf => new TransitionResult(this, jsMethod: inf.Method, jsParams: inf.Params, jsJingle: inf.Jingle.ToString()));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White} contestants".Color(null).Fmt(Contestants.Count);
            }
        }

        public override string JsMethod { get { return "setup"; } }
        public override object JsParameters { get { return null; } }
        public override string JsMusic { get { return CurrentMusic?.ToString(); } }
        public override string JsJingle { get { return CurrentJingle?.ToString(); } }
    }
}
