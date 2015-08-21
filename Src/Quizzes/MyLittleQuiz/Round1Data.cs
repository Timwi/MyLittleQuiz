using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round1Data : ICloneable
    {
        public QuizData QuizData { get; private set; }
        public Round1Contestant[] Contestants { get; private set; }
        public int NumContestantsNeeded { get; private set; }
        public Dictionary<Difficulty, int> QuestionIndex { get; private set; }
        [ClassifyIgnoreIfDefault]
        public int? SelectedContestant { get; private set; }
        [ClassifyIgnoreIfDefault]
        public Difficulty? CurrentDifficulty { get; private set; }
        [ClassifyIgnoreIfDefault]
        public object AnswerObject { get; private set; }

        public Round1Data(QuizData quizData, IEnumerable<Contestant> contestants)
        {
            QuizData = quizData;
            Contestants = contestants.Select(c => new Round1Contestant(c.Name, c.Roll)).ToArray();
            NumContestantsNeeded = 10;
            QuestionIndex = new Dictionary<Difficulty, int> { { Difficulty.Easy, 0 }, { Difficulty.Medium, 0 } };
            SelectedContestant = null;
            CurrentDifficulty = null;
            AnswerObject = null;
        }

        private Round1Data() { }    // for Classify

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Dictionary<Difficulty, QuestionBase[]> Questions { get { return QuizData.Round1Questions; } }

        public ConsoleColoredString Describe
        {
            get
            {
                var numOut = Contestants.Count(c => c.NumWrong > 1);
                var numThrough = Contestants.Count(c => c.NumCorrect > 1);
                var numRemaining = Contestants.Count(c => c.NumCorrect < 2 && c.NumWrong < 2);

                return "{0/White}\n{1}\n{2}{3}\n\n{4/White}\n{5}".Color(null).Fmt(
                    /* 0 */ "Contestants:",
                    /* 1 */ Contestants.SelectIndexWhere(c => c.NumCorrect < 2 && c.NumWrong < 2).Select(i =>
                                    (i == SelectedContestant ? "[".Color(ConsoleColor.Magenta, ConsoleColor.DarkRed) : null) +
                                    "•".Color(ConsoleColor.White, ConsoleColor.DarkGreen).Repeat(Contestants[i].NumCorrect).JoinColoredString() +
                                    "•".Color(ConsoleColor.Black, ConsoleColor.Red).Repeat(Contestants[i].NumWrong).JoinColoredString() +
                                    (i + 1).ToString().Color(ConsoleColor.White, i == SelectedContestant ? ConsoleColor.DarkRed : (ConsoleColor?) null) +
                                    (i == SelectedContestant ? "]".Color(ConsoleColor.Magenta, ConsoleColor.DarkRed) : null))
                                    .JoinColoredString(" "),
                    /* 2 */ "{0/Red} out, {1/Green} through, {2/Cyan} remaining, need {3/Yellow} more through or {4/Magenta} more out".Color(null).Fmt(numOut, numThrough, numRemaining, NumContestantsNeeded - numThrough, numThrough + numRemaining - NumContestantsNeeded),
                    /* 3 */ SelectedContestant == null ? null : "\n\n{0/White}\n{1/Green} {2/Red} {3/Yellow}".Color(null).Fmt("Selected contestant:", Contestants[SelectedContestant.Value].NumCorrect, Contestants[SelectedContestant.Value].NumWrong, Contestants[SelectedContestant.Value].Name),
                    /* 4 */ "Questions:",
                    /* 5 */ Questions.Select(kvp => "{0/Cyan}: {1/Magenta}".Color(null).Fmt(kvp.Key, kvp.Value.Length)).JoinColoredString("\n")
                );
            }
        }

        public Round1Data SelectContestant(int index)
        {
            return this.ApplyToClone(c => { c.SelectedContestant = index; });
        }

        public Round1Data AskQuestion(Difficulty difficulty)
        {
            return this.ApplyToClone(c => { c.CurrentDifficulty = difficulty; });
        }

        public Round1Data GiveAnswer(object answerObj)
        {
            return this.ApplyToClone(c => { c.AnswerObject = answerObj; });
        }

        public Round1Data DismissQuestion()
        {
            return this.ApplyToClone(cl =>
            {
                // Take a copy of this dictionary before modifying it
                cl.QuestionIndex = new Dictionary<Difficulty, int>(QuestionIndex);
                cl.QuestionIndex[CurrentDifficulty.Value] = QuestionIndex[CurrentDifficulty.Value] + 1;

                // Update the contestant’s score
                cl.Contestants = Contestants.ReplaceIndex(cl.SelectedContestant.Value, c => c.IncScore(!AnswerObject.Equals(false)));

                cl.SelectedContestant = null;
                cl.CurrentDifficulty = null;
                cl.AnswerObject = null;
            });
        }
    }
}
