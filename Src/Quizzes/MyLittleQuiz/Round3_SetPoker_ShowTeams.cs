using System;
using System.Collections.Generic;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

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
                if (Data.TeamA.Score > 1 || Data.TeamB.Score > 1)
                    yield return Transition.Simple(ConsoleKey.N, "Next round: Final",
                        () => new Round4_Final_ShowContestants(new Round4Data(Data.QuizData, (Data.TeamA.Score > 1 ? Data.TeamA : Data.TeamB).Contestants)));

                else if (Data.SetIndex >= 0 && Data.SetIndex < Data.QuizData.Round3Sets.Length)
                    yield return Transition.Simple(ConsoleKey.S, "Show set: " + Data.QuizData.Round3Sets[Data.SetIndex].Name,
                        () => new Round3_SetPoker_ShowSet(Data.InitSet()));

                if (Data.SetIndex > 0)
                    yield return Transition.Simple(ConsoleKey.R, "Reveal remaining answers for previous set ({0})".Fmt(Data.QuizData.Round3Sets[Data.SetIndex - 1].Name),
                        () => new Round3_SetPoker_Reveal(Data));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\nTeam A: {1/Cyan}\nTeam B: {2/Cyan}".Color(ConsoleColor.Yellow)
                    .Fmt("Score:", Data.TeamA.Score, Data.TeamB.Score);
            }
        }

        public override string JsMethod { get { return "r3_showTeams"; } }
        public override string JsMusic { get { return Data.MusicStarted ? "music3" : null; } }
        public override object JsParameters
        {
            get { return new { teamA = Data.TeamA, teamB = Data.TeamB }; }
        }
    }
}
