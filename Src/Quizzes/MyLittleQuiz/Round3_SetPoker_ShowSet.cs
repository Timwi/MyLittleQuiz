using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round3_SetPoker_ShowSet : QuizStateBase
    {
        public Round3Data Data { get; private set; }

        public Round3_SetPoker_ShowSet(Round3Data data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            Data = data;
        }

        private Round3_SetPoker_ShowSet() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.P, "Play", () => Data.SetIndex == 2 ? (QuizStateBase) new Round3_SetPoker_PlayTieBreaker(Data) : new Round3_SetPoker_Play(Data));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\nTeam A: {1/Cyan}\nTeam B: {2/Cyan}\n\n{3/White} {4/Magenta}\n\n{5/Green}".Color(ConsoleColor.Yellow).Fmt(
                    /* 0 */ "Score:",
                    /* 1 */ Data.TeamA.HasPoint ? 1 : 0,
                    /* 2 */ Data.TeamB.HasPoint ? 1 : 0,
                    /* 3 */ "Current set:",
                    /* 4 */ Data.QuizData.Round3Sets[Data.SetIndex].Name,
                    /* 5 */ Data.SetIndex == 2 ? "Tie Breaker" : "Regular");
            }
        }

        public override string JsMethod
        {
            get { return "r3_showTeams"; }
        }

        public override object JsParameters
        {
            get { return new { teamA = Data.TeamA, teamB = Data.TeamB }; }
        }
    }
}
