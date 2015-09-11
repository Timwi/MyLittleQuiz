using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round3_SetPoker_ShowTeams : QuizStateBase
    {
        public Round3Data Data { get; private set; }

        public Round3_SetPoker_ShowTeams(Round3Data data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            Data = data;
        }

        private Round3_SetPoker_ShowTeams() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                if (Data.SetIndex >= 0 && Data.SetIndex < Data.QuizData.Round3Sets.Length)
                    yield return Transition.Simple(ConsoleKey.S, "Show set: " + Data.QuizData.Round3Sets[Data.SetIndex].Name,
                        () => new Round3_SetPoker_ShowSet(Data));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\nTeam A: {1/Cyan}\nTeam B: {2/Cyan}".Color(ConsoleColor.Yellow)
                    .Fmt("Score:", Data.TeamA.HasPoint ? 1 : 0, Data.TeamB.HasPoint ? 1 : 0);
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
