using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;
using RT.Util.Text;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round4Data : ICloneable
    {
        public const int MinQuestions = 5;

        public QuizData QuizData { get; private set; }
        [ClassifyNotNull]
        public ContestantAndScore[] Contestants { get; private set; } = new ContestantAndScore[0];
        [ClassifyNotNull]
        public bool[][] Answers { get; private set; } = new bool[0][];
        public int QuestionIndex { get; private set; }
        [ClassifyIgnoreIfDefault]
        public object AnswerObject { get; private set; }

        public Round4Data(QuizData quizData, ContestantAndScore[] contestants)
        {
            QuizData = quizData;
            Contestants = contestants;
            Answers = Ut.NewArray(contestants.Length, i => new bool[0]);
            QuestionIndex = 0;
            AnswerObject = null;
        }

        private Round4Data() { }    // for Classify

        public object Clone()
        {
            return MemberwiseClone();
        }

        public QuestionBase[] Questions { get { return QuizData.Round4Questions; } }
        public QuestionBase CurrentQuestion { get { return QuizData.Round4Questions[QuestionIndex]; } }

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

        public Round4Data GiveAnswer(object answer)
        {
            var turn = WhoseTurn;
            var correct = !Equals(answer, false);
            return this.ApplyToClone(r4c =>
            {
                r4c.Answers = Answers.Select((ans, ix) => ix == turn ? ans.Concat(correct).ToArray() : ans).ToArray();
                r4c.AnswerObject = answer;
            });
        }

        public Round4Data DismissQuestion()
        {
            return this.ApplyToClone(r4c =>
            {
                r4c.AnswerObject = null;
                r4c.QuestionIndex = QuestionIndex + 1;
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
                var cols = Math.Max(MinQuestions, Answers.Max(a => a.Length));
                if (Answers[0].Length >= MinQuestions && whoseTurn == 0)
                    cols++;
                var cOut = ContestantsOutOfGame;
                for (int i = 0; i < Contestants.Length; i++)
                {
                    if (cOut.Contains(i))
                        tt.SetCell(0, i, "out".Color(ConsoleColor.Red));
                    else if (i == whoseTurn)
                        tt.SetCell(0, i, "▶".Color(ConsoleColor.Yellow));
                    tt.SetCell(1, i, Contestants[i].Name.Color(ConsoleColor.White));
                    for (int j = 0; j < cols; j++)
                        tt.SetCell(j + 2, i, j >= Answers[i].Length ? "?".Color(ConsoleColor.Blue) : Answers[i][j] ? "✓".Color(ConsoleColor.Green) : "✗".Color(ConsoleColor.Magenta));
                }
                return tt.ToColoredString();
            }
        }

        public int[] ContestantsOutOfGame
        {
            get
            {
                var numQuestions = Answers[0].Length.ClipMin(MinQuestions);
                var potentialPoints = Answers.Select(ans => ans.Count(b => b) + (numQuestions - ans.Length).ClipMin(0)).ToArray();
                var maxPoints = Answers.Select(a => a.Count(b => b)).Max();
                return potentialPoints.SelectIndexWhere(pp => pp < maxPoints).ToArray();
            }
        }
    }
}
