using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util.Consoles;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public abstract class Round3_SetPoker_PlayBase : QuizStateBase
    {
        public Round3Data Data { get; private set; }
        [ClassifyNotNull]
        public string[] AnswersGiven { get; private set; }

        public Round3Set CurrentSet { get { return Data.QuizData.Round3Sets[Data.SetIndex]; } }

        public Round3_SetPoker_PlayBase(Round3Data data, string[] answersGiven = null, bool[] answersCorrect = null)
        {
            Data = data;
            AnswersGiven = answersGiven ?? new string[0];
        }

        protected Round3_SetPoker_PlayBase() { }  // for Classify

        public abstract Round3_SetPoker_PlayBase GiveAnswer(string answer);

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                var remaining = CurrentSet.Answers.Except(AnswersGiven).ToArray();

                yield return Transition.Find(ConsoleKey.G, "Give an answer", remaining,
                    ans => ans.Color(ans == null ? ConsoleColor.Red : ConsoleColor.Green),
                    ans => GiveAnswer(ans));

                yield return Transition.Simple(ConsoleKey.M, "Give a wrong answer", "r3_showWrong");

                yield return Transition.Simple(ConsoleKey.L, "List {0} remaining correct answers".Fmt(remaining.Length), () =>
                {
                    foreach (var rem in remaining)
                        Console.WriteLine(rem);
                    Program.ReadKey();
                });

                yield return Transition.Simple(ConsoleKey.A, "Team A wins", () => new Round3_SetPoker_ShowTeams(Data.TeamAWins()));
                yield return Transition.Simple(ConsoleKey.B, "Team B wins", () => new Round3_SetPoker_ShowTeams(Data.TeamBWins()));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "Current Set: {0/Yellow}".Color(ConsoleColor.Cyan).Fmt(CurrentSet.Name);
            }
        }

        public override string JsMethod
        {
            get { return "r3_play"; }
        }
    }
}
