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
        [ClassifyNotNull]
        public bool[] AnswersCorrect { get; private set; }

        public Round3Set CurrentSet { get { return Data.QuizData.Round3Sets[Data.SetIndex]; } }
        public IEnumerable<string> CorrectAnswersGiven { get { return AnswersGiven.Where((a, i) => AnswersCorrect[i]); } }

        public Round3_SetPoker_PlayBase(Round3Data data, string[] answersGiven = null, bool[] answersCorrect = null)
        {
            Data = data;
            AnswersGiven = answersGiven ?? new string[0];
            AnswersCorrect = answersCorrect ?? new bool[0];
        }

        protected Round3_SetPoker_PlayBase() { }  // for Classify

        public abstract Round3_SetPoker_PlayBase GiveAnswer(string answer, bool correct);

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                var remaining = CurrentSet.Answers.Except(CorrectAnswersGiven).ToArray();

                yield return Transition.Find(ConsoleKey.G, "Give an answer", remaining.Concat((string) null),
                    ans => (ans ?? "<wrong answer>").Color(ans == null ? ConsoleColor.Red : ConsoleColor.Green),
                    ans =>
                    {
                        if (ans != null)
                            return GiveAnswer(ans, true);
                        var wrongAnswer = InputBox.GetLine("Wrong answer?", caption: "Give wrong answer");
                        if (wrongAnswer == null)
                            return null;
                        return GiveAnswer(wrongAnswer, false);
                    });

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

        public override object JsParameters
        {
            get { return new { answers = AnswersGiven, correct = AnswersCorrect }; }
        }
    }
}
