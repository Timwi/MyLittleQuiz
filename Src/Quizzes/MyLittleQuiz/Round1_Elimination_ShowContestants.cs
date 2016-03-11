using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round1_Elimination_ShowContestants : QuizStateBase
    {
        public Round1Data Data { get; private set; }

        public Round1_Elimination_ShowContestants(Round1Data data) { Data = data; }
        private Round1_Elimination_ShowContestants() { } // for Classify

        public static TransitionResult TransitionTo(Round1Data data)
        {
            var through = data.Contestants.Where(c => c.IsThrough).ToArray();
            var throughAndRemaining = data.Contestants.Where(c => c.IsThrough || c.IsStillInGame).ToArray();
            var nextRoundContestants =
                through.Length == data.QuizData.Round1NumContestantsNeeded ? through :
                throughAndRemaining.Length == data.QuizData.Round1NumContestantsNeeded ? throughAndRemaining : null;

            if (nextRoundContestants != null)
                return new Round2_Categories_ShowContestants(new Round2Data(data.QuizData, (Round2Contestant[]) nextRoundContestants.Select(c => new Round2Contestant(c.Name, 0)).ToArray().Shuffle()), noScores: true)
                    .With(jsJingle: Jingle.Tada.ToString());

            return new Round1_Elimination_ShowContestants(data).With();
        }

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.SelectIndex(ConsoleKey.S, "Select contestant", Data.Contestants, index => new Round1_Elimination_ShowContestants(Data.SelectContestant(index)));

                yield return Transition.Simple(ConsoleKey.R, "Select contestant at random", () =>
                {
                    var choosableContestants = Data.Contestants.SelectIndexWhere(c => c.IsStillInGame).ToArray();
                    var index = choosableContestants[Rnd.Next(choosableContestants.Length)];
                    return new Round1_Elimination_ShowContestants(Data.SelectContestant(index));
                });

                if (Data.SelectedContestant != null)
                {
                    yield return Transition.Simple(ConsoleKey.Q, "Ask the question", () => new Round1_Elimination_Q(Data.StartMusic()));
                    yield return Transition.Simple(ConsoleKey.D, "Disqualify", () => TransitionTo(Data.DisqualifySelectedContestant()));
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

        public override string JsMusic { get { return Data.MusicStarted ? Music.Music1.ToString() : null; } }
        public override string JsMethod { get { return Data.SelectedContestant == null ? "r1_showContestants" : "r1_select"; } }
        public override string JsJingle { get { return Data.SelectedContestant == null ? null : Jingle.Swoosh.ToString(); } }
        public override object JsParameters
        {
            get
            {
                if (Data.SelectedContestant == null)
                    return new
                    {
                        contestants = Data.Contestants.Select((c, i) => new
                        {
                            Name = c.Name,
                            Roll = c.Roll,
                            Number = i + 1,
                            HasCorrect = c.NumCorrect > 0,
                            HasWrong = c.NumWrong > 0,
                            Shown = c.IsStillInGame
                        }).Where(inf => inf.Shown).ToArray()
                    };
                return new { contestant = Data.Contestants[Data.SelectedContestant.Value] };
            }
        }
    }
}
