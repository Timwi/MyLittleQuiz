using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace Trophy.MyLittleQuiz
{
    public abstract class Round3_SetPoker_PlayBase : QuizStateBase
    {
        public Round3Data Data { get; private set; }

        public Round3Set CurrentSet { get { return Data.QuizData.Round3Sets[Data.SetIndex]; } }

        public Round3_SetPoker_PlayBase(Round3Data data)
        {
            Data = data;
        }

        protected Round3_SetPoker_PlayBase() { }  // for Classify

        public abstract Round3_SetPoker_PlayBase GiveCorrectAnswer(string answer);
        public abstract TransitionResult GiveWrongAnswer();

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                var remaining = CurrentSet.Answers.Except(Data.AnswersGiven).ToArray();

                yield return Transition.Find(ConsoleKey.G, "Give a correct answer", remaining,
                    ans => ans.Color(ans == null ? ConsoleColor.Red : ConsoleColor.Green),
                    ans => GiveCorrectAnswer(ans).With("r3_play_transition", jsJingle: Jingle.Round3CorrectAnswer.ToString()));

                yield return Transition.Simple(ConsoleKey.M, "Give a wrong answer", () => GiveWrongAnswer());

                yield return Transition.Simple(ConsoleKey.L, "List {0} remaining correct answers".Fmt(remaining.Length), () =>
                {
                    foreach (var rem in remaining)
                        Console.WriteLine(rem);
                    ReadKey();
                });

                yield return Transition.Simple(ConsoleKey.A, "Team A wins", () => new Round3_SetPoker_ShowTeams(Data.TeamAWins()).With(jsJingle: Jingle.Tada.ToString()));
                yield return Transition.Simple(ConsoleKey.B, "Team B wins", () => new Round3_SetPoker_ShowTeams(Data.TeamBWins()).With(jsJingle: Jingle.Tada.ToString()));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "Current Set: {0/Yellow}".Color(ConsoleColor.Cyan).Fmt(CurrentSet.Name);
            }
        }

        public override string JsMethod { get { return "r3_play"; } }
        public override string JsMusic { get { return Data.MusicStarted ? Music.Music3.ToString() : null; } }
    }
}
