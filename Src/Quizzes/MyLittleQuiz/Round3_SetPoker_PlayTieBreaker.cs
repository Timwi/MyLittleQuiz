using System;
using System.Linq;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using RT.Util.Consoles;
using RT.Util.Dialogs;
using RT.Util.Text;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round3_SetPoker_PlayTieBreaker : Round3_SetPoker_PlayBase
    {
        public bool TeamAStarted { get; private set; }

        public Round3_SetPoker_PlayTieBreaker(Round3Data data, string[] answersGiven, bool teamAStarted)
            : base(data, answersGiven)
        {
            TeamAStarted = teamAStarted;
        }

        private Round3_SetPoker_PlayTieBreaker() { }    // for Classify

        public override Round3_SetPoker_PlayBase GiveAnswer(string answer)
        {
            return new Round3_SetPoker_PlayTieBreaker(Data, AnswersGiven.Concat(answer).ToArray(), TeamAStarted);
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
                for (int i = 0; i < AnswersGiven.Length + 1; i++)
                {
                    teamAsTurn = (i % 2 != 0) ^ TeamAStarted;
                    tt.SetCell(teamAsTurn ? 0 : 1, i / 2 + 1, i == AnswersGiven.Length ? "NEXT".Color(ConsoleColor.Magenta) : AnswersGiven[i].Color(teamAsTurn ? ConsoleColor.Yellow : ConsoleColor.Green));
                }

                return tt.ToColoredString() + "\n\n{0}’s turn".Color(null).Fmt(teamAsTurn ? "Team A".Color(ConsoleColor.Yellow) : "Team B".Color(ConsoleColor.Green));
            }
        }

        public override object JsParameters
        {
            get { return new { tie = true, answers = AnswersGiven, teamAStarted = TeamAStarted }; }
        }
    }
}
