using System;
using System.Linq;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;
using RT.Util.Text;

namespace Trophy.MyLittleQuiz
{
    public sealed class Round4Data : ICloneable
    {
        public QuizData QuizData { get; private set; }
        [ClassifyNotNull]
        public ContestantAndScore[] Contestants { get; private set; } = new ContestantAndScore[0];
        [ClassifyNotNull]
        public bool[][] Answers { get; private set; } = new bool[0][];
        [ClassifyNotNull]
        public int[] QuestionsTaken { get; private set; } = new int[0];
        public int CurrentQuestionIndex { get; private set; }
        [ClassifyIgnoreIfDefault]
        public bool? AnswerGiven { get; private set; }

        public bool MusicStarted { get; private set; }
        public Round4Data StartMusic()
        {
            return this.ApplyToClone(c => { c.MusicStarted = true; });
        }

        public Round4Data(QuizData quizData, ContestantAndScore[] contestants)
        {
            QuizData = quizData;
            Contestants = contestants;
            Answers = Ut.NewArray(contestants.Length, i => new bool[0]);
            chooseQuestion();
            AnswerGiven = null;
        }

        private void chooseQuestion()
        {
            var available = Enumerable.Range(0, Questions.Length).Except(QuestionsTaken).ToArray();
            if (available.Length == 0)
                CurrentQuestionIndex = 0;
            else
                CurrentQuestionIndex = available[Rnd.Next(0, available.Length)];
        }

        private Round4Data() { }    // for Classify

        public object Clone()
        {
            return MemberwiseClone();
        }

        public QuestionBase[] Questions { get { return QuizData.Round4Questions; } }
        public QuestionBase CurrentQuestion { get { return QuizData.Round4Questions[CurrentQuestionIndex]; } }

        public int WhoseTurn
        {
            get
            {
                var len = Answers[0].Length;
                for (int i = 1; i < Answers.Length; i++)
                    if (Answers[i].Length < len)
                        return i;
                return 0;
            }
        }

        public Round4Data GiveAnswer(bool correct)
        {
            var turn = WhoseTurn;
            return this.ApplyToClone(r4c =>
            {
                r4c.Answers = Answers.Select((ans, ix) => ix == turn ? ans.Concat(correct).ToArray() : ans).ToArray();
                r4c.AnswerGiven = correct;
            });
        }

        public Round4Data DismissQuestion()
        {
            return this.ApplyToClone(r4c =>
            {
                r4c.AnswerGiven = null;
                r4c.QuestionsTaken = QuestionsTaken.Concat(CurrentQuestionIndex).ToArray();
                r4c.chooseQuestion();
            });
        }

        public Round4Data RemoveContestant(int index)
        {
            return this.ApplyToClone(r4c =>
            {
                r4c.Contestants = Contestants.RemoveIndex(index);
                r4c.Answers = Answers.RemoveIndex(index);
            });
        }

        public ConsoleColoredString Describe
        {
            get
            {
                var whoseTurn = WhoseTurn;
                var tt = new TextTable { ColumnSpacing = 2 };
                var cols = Math.Max(QuizData.Round4MinQuestions, Answers.Max(a => a.Length));
                if (Answers[0].Length >= QuizData.Round4MinQuestions && whoseTurn == 0)
                    cols++;
                var cOut = ContestantsOutOfGame;
                for (int i = 0; i < Contestants.Length; i++)
                {
                    if (cOut.Contains(i))
                        tt.SetCell(0, i, "out".Color(ConsoleColor.Red));
                    else if (i == whoseTurn)
                        tt.SetCell(0, i, "▶".Color(ConsoleColor.Yellow));
                    tt.SetCell(1, i, Contestants[i].Score.ToString().Color(ConsoleColor.DarkYellow), alignment: HorizontalTextAlignment.Right);
                    tt.SetCell(2, i, Contestants[i].Name.Color(ConsoleColor.White));
                    for (int j = 0; j < cols; j++)
                        tt.SetCell(j + 3, i, j >= Answers[i].Length ? "?".Color(ConsoleColor.Blue) : Answers[i][j] ? "✓".Color(ConsoleColor.Green) : "✗".Color(ConsoleColor.Magenta));
                }
                return "{0}\n{1}".Color(null).Fmt(
                    tt.ToColoredString(),
                    QuestionsTaken.Length >= Questions.Length
                        ? "NO MORE QUESTIONS".Color(ConsoleColor.Red)
                        : "{0/Yellow} questions left.\n\n{1}".Color(ConsoleColor.Green).Fmt(Questions.Length - QuestionsTaken.Length, CurrentQuestion.Describe(AnswerGiven)));
            }
        }

        public int[] ContestantsOutOfGame
        {
            get
            {
                var numQuestions = Answers[0].Length.ClipMin(QuizData.Round4MinQuestions);
                var potentialPoints = Answers.Select(ans => ans.Count(b => b) + (numQuestions - ans.Length).ClipMin(0)).ToArray();
                var maxPoints = Answers.Select(a => a.Count(b => b)).Max();
                return potentialPoints.SelectIndexWhere(pp => pp < maxPoints).ToArray();
            }
        }
    }
}
