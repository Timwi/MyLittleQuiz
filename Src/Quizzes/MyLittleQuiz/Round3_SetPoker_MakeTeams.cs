using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round3_SetPoker_MakeTeams : QuizStateBase
    {
        public QuizData QuizData { get; private set; }
        public ContestantAndScore[] TeamA { get; private set; }
        public ContestantAndScore[] UnassignedContestants { get; private set; }

        public Round3_SetPoker_MakeTeams(QuizData quizData, ContestantAndScore[] contestants)
        {
            if (contestants == null)
                throw new ArgumentNullException("contestants");

            QuizData = quizData;
            TeamA = new ContestantAndScore[0];
            UnassignedContestants = contestants;
        }
        private Round3_SetPoker_MakeTeams() { } // for Classify

        public Round3_SetPoker_MakeTeams AssignContestantToTeamA(int index)
        {
            return this.ApplyToClone(r3 =>
            {
                r3.TeamA = TeamA.Concat(UnassignedContestants[index]).ToArray();
                r3.UnassignedContestants = UnassignedContestants.RemoveIndex(index);
            });
        }

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                foreach (var transition in UnassignedContestants.Select((cont, index) =>
                    Transition.Simple(ConsoleKey.A + index, "Assign {0} to Team A".Fmt(cont.Name), () => AssignContestantToTeamA(index).NoTransition())))
                    yield return transition;

                yield return Transition.Simple(ConsoleKey.I, "Intro Round 3", "r3_intro", jsJingle: Jingle.Round3Start.ToString());

                if (TeamA.Length == UnassignedContestants.Length)
                    yield return Transition.Simple(ConsoleKey.Z, "Start Round 3", () => new Round3_SetPoker_ShowTeams(new Round3Data(QuizData, TeamA, UnassignedContestants)));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\n{1/Yellow}".Color(null).Fmt(
                    "Team A:",
                    TeamA.Select(c => c.Name).JoinString("\n"));
            }
        }

        public override string JsMethod { get { return "r3_showContestants"; } }
        public override string JsMusic { get { return null; } }
        public override object JsParameters { get { return new { contestants = TeamA.Concat(UnassignedContestants).ToArray() }; } }
    }
}
