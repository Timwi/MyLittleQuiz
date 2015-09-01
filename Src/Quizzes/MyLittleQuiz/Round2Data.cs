using System;
using System.Linq;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;
using RT.Util.Text;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round2Data : ICloneable
    {
        public QuizData QuizData { get; private set; }
        public Round2Contestant[] Contestants { get; private set; }
        public int CurrentContestant { get; private set; }
        public bool[][] QuestionsUsed { get; private set; }
        public int? NextCategoryToPresent { get; private set; }

        [ClassifyIgnoreIfDefault]
        public int? SelectedCategory { get; private set; }
        [ClassifyIgnoreIfDefault]
        public int? SelectedQuestion { get; private set; }
        [ClassifyIgnoreIfDefault]
        public object AnswerObject { get; private set; }

        public Round2Category[] Categories { get { return QuizData.Round2Categories; } }

        public Round2Data(QuizData quizData, Round2Contestant[] contestants)
        {
            if (quizData == null)
                throw new ArgumentNullException("data");
            if (contestants == null)
                throw new ArgumentNullException("contestants");

            QuizData = quizData;
            Contestants = contestants;
            QuestionsUsed = quizData.Round2Categories.Select(cat => cat.Questions.Select(q => false).ToArray()).ToArray();
            SelectedCategory = null;
            SelectedQuestion = null;
            CurrentContestant = 0;
            NextCategoryToPresent = 0;
        }

        private Round2Data() { }    // for Classify

        public Round2Data SelectCategory(int cat) { return this.ApplyToClone(r2d => { r2d.SelectedCategory = cat; }); }
        public Round2Data SelectQuestion(int index) { return this.ApplyToClone(r2d => { r2d.SelectedQuestion = index; }); }
        public Round2Data GiveAnswer(object answer) { return this.ApplyToClone(r2d => { r2d.AnswerObject = answer; }); }

        public Round2Data DismissQuestion()
        {
            return this.ApplyToClone(r2d =>
            {
                r2d.QuestionsUsed = QuestionsUsed.ReplaceIndex(SelectedCategory.Value, qu => qu.ReplaceIndex(SelectedQuestion.Value, true));
                r2d.Contestants = Contestants.ReplaceIndex(CurrentContestant, c => c.IncScore((SelectedQuestion.Value + 1) * (AnswerObject.Equals(false) ? -1 : 1)));
                r2d.SelectedCategory = null;
                r2d.SelectedQuestion = null;
                r2d.AnswerObject = null;
                r2d.CurrentContestant = (CurrentContestant + 1) % Contestants.Length;
            });
        }

        public Round2Data PresentCategory()
        {
            return this.ApplyToClone(r2d =>
            {
                r2d.NextCategoryToPresent =
                    NextCategoryToPresent == null ? null :
                    NextCategoryToPresent == Categories.Length - 1 ? null :
                    (int?) (NextCategoryToPresent.Value + 1);
            });
        }

        public ConsoleColoredString Describe
        {
            get
            {
                var tt = new TextTable { ColumnSpacing = 2 };
                var row = 0;

                tt.SetCell(1, row, "CONTESTANTS".Color(ConsoleColor.White), alignment: HorizontalTextAlignment.Center, background: ConsoleColor.DarkGreen, colSpan: 4);
                tt.SetCell(7, row, "CATEGORIES".Color(ConsoleColor.White), alignment: HorizontalTextAlignment.Center, background: ConsoleColor.DarkGreen, colSpan: 7);

                row++;

                tt.SetCell(1, row, "Index".Color(ConsoleColor.White), alignment: HorizontalTextAlignment.Right);
                tt.SetCell(2, row, "Contestant".Color(ConsoleColor.White));
                tt.SetCell(3, row, "Score".Color(ConsoleColor.White), alignment: HorizontalTextAlignment.Right);
                tt.SetCell(4, row, "Rank".Color(ConsoleColor.White), alignment: HorizontalTextAlignment.Right);

                tt.SetCell(5, row, "•");

                tt.SetCell(7, row, "Index".Color(ConsoleColor.White), alignment: HorizontalTextAlignment.Right);
                tt.SetCell(8, row, "Category".Color(ConsoleColor.White));
                tt.SetCell(9, row, "VE".Color(ConsoleColor.White));
                tt.SetCell(10, row, "Ea".Color(ConsoleColor.White));
                tt.SetCell(11, row, "Me".Color(ConsoleColor.White));
                tt.SetCell(12, row, "Ha".Color(ConsoleColor.White));
                tt.SetCell(13, row, "VH".Color(ConsoleColor.White));

                row++;

                for (int i = 0; i < Math.Max(Contestants.Length, Categories.Length); i++)
                {
                    ConsoleColor? bg = null;
                    if (i < Contestants.Length)
                    {
                        if (i == CurrentContestant)
                        {
                            tt.SetCell(0, row, "▶");
                            bg = ConsoleColor.DarkBlue;
                        }
                        tt.SetCell(1, row, (i + 1).ToString().Color(ConsoleColor.Green), alignment: HorizontalTextAlignment.Right, background: bg);
                        tt.SetCell(2, row, Contestants[i].Name.Color(ConsoleColor.Yellow), background: bg);
                        tt.SetCell(3, row, Contestants[i].Score.ToString().Color(ConsoleColor.Cyan), alignment: HorizontalTextAlignment.Right, background: bg);
                        tt.SetCell(4, row, (Contestants.Count(c => c.Score > Contestants[i].Score) + 1).ToString().Color(ConsoleColor.Magenta), alignment: HorizontalTextAlignment.Right, background: bg);
                    }

                    if (i < Categories.Length)
                    {
                        bg = null;
                        if (i == SelectedCategory)
                        {
                            tt.SetCell(6, row, "▶");
                            bg = ConsoleColor.DarkBlue;
                        }
                        var dark = NextCategoryToPresent != null && NextCategoryToPresent.Value <= i;
                        tt.SetCell(7, row, (i + 1).ToString().Color(dark ? ConsoleColor.DarkGreen : ConsoleColor.Green), alignment: HorizontalTextAlignment.Right, background: bg);
                        tt.SetCell(8, row, Categories[i].Name.Color(dark ? ConsoleColor.DarkYellow : ConsoleColor.Yellow), background: bg);
                        for (int j = 0; j < 5; j++)
                            tt.SetCell(9 + j, row, j >= QuestionsUsed[i].Length ? "?".Color(dark ? ConsoleColor.DarkRed : ConsoleColor.Red) : QuestionsUsed[i][j] ? "" : "█".Color(dark ? ConsoleColor.DarkCyan : ConsoleColor.Cyan), background: bg);
                    }

                    row++;
                }
                return tt.ToColoredString();
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
