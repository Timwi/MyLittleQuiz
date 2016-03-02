using System;
using System.Collections.Generic;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Text;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round3_SetPoker_PlayTieBreaker : Round3_SetPoker_PlayBase
    {
        public bool TeamAStarted { get; private set; }

        public Round3_SetPoker_PlayTieBreaker(Round3Data data, bool teamAStarted)
            : base(data)
        {
            TeamAStarted = teamAStarted;
        }

        private Round3_SetPoker_PlayTieBreaker() { }    // for Classify

        public override Round3_SetPoker_PlayBase GiveCorrectAnswer(string answer)
        {
            return new Round3_SetPoker_PlayTieBreaker(Data.GiveCorrectAnswer(answer), TeamAStarted);
        }

        public override Round3_SetPoker_PlayBase GiveWrongAnswer()
        {
            return new Round3_SetPoker_PlayTieBreaker(Data.GiveWrongAnswer(true), TeamAStarted);
        }

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                if (Data.AnswersGiven.TakeLast(2).SequenceEqual(new string[] { null, null }))
                    return removeStrikes();
                return base.Transitions;
            }
        }

        private IEnumerable<Transition> removeStrikes()
        {
            yield return Transition.Simple(ConsoleKey.R, "Remove strikes", () => new Round3_SetPoker_PlayTieBreaker(Data.RemoveStrikes(), TeamAStarted));
        }

        public bool IsTeamAsTurn
        {
            get
            {
                return (Data.AnswersGiven.Length % 2 != 0) ^ TeamAStarted;
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                var tt = new TextTable { ColumnSpacing = 2 };
                tt.SetCell(0, 0, "Team A".Color(ConsoleColor.Yellow));
                tt.SetCell(1, 0, "Team B".Color(ConsoleColor.Green));
                tt.SetRowBackground(0, ConsoleColor.DarkBlue);
                var teamAsTurn = false;
                for (int i = 0; i < Data.AnswersGiven.Length + 1; i++)
                {
                    teamAsTurn = (i % 2 != 0) ^ TeamAStarted;
                    tt.SetCell(teamAsTurn ? 0 : 1, i / 2 + 1,
                        i == Data.AnswersGiven.Length ? "NEXT".Color(ConsoleColor.Magenta) :
                        Data.AnswersGiven[i] == null ? "<wrong>".Color(ConsoleColor.Red) :
                        Data.AnswersGiven[i].Color(teamAsTurn ? ConsoleColor.Yellow : ConsoleColor.Green));
                }

                return tt.ToColoredString() + "\n\n{0}’s turn".Color(null).Fmt(teamAsTurn ? "Team A".Color(ConsoleColor.Yellow) : "Team B".Color(ConsoleColor.Green));
            }
        }

        public override object JsParameters
        {
            get
            {
                return new
                {
                    tie = true,
                    answers = Data.AnswersGiven,
                    teamAStarted = TeamAStarted
                };
            }
        }
    }
}
