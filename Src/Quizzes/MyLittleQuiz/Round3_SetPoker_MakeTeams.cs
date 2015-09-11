﻿using System;
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
        public string[] TeamA { get; private set; }
        public string[] UnassignedContestants { get; private set; }

        public Round3_SetPoker_MakeTeams(QuizData quizData, string[] contestants)
        {
            if (contestants == null)
                throw new ArgumentNullException("contestants");

            QuizData = quizData;
            TeamA = new string[0];
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
                    Transition.Simple((ConsoleKey) (ConsoleKey.A + index), "Assign {0} to Team A".Fmt(cont), () => AssignContestantToTeamA(index))))
                    yield return transition;

                if (TeamA.Length == UnassignedContestants.Length)
                    yield return Transition.Simple(ConsoleKey.Z, "Done", () => new Round3_SetPoker_ShowTeams(new Round3Data(QuizData, TeamA, UnassignedContestants)));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\n{1/Yellow}".Color(null).Fmt(
                    "Team A:",
                    TeamA.JoinString("\n"));
            }
        }

        public override string JsMethod { get { return null; } }
        public override object JsParameters { get { return null; } }
    }
}