using System;
using System.Linq;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;
using RT.Util.Text;

namespace Trophy.MyLittleQuiz
{
    public sealed class Round2Data : ICloneable, IClassifyObjectProcessor
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
        public bool? AnswerGiven { get; private set; }

        public Round2Category[] Categories { get { return QuizData.Round2Categories; } }
        public int NumContestantsNeeded { get { return QuizData.Round2NumContestantsNeeded; } }

        public bool MusicStarted { get; private set; }
        public Round2Data StartMusic()
        {
            return this.ApplyToClone(c => { c.MusicStarted = true; });
        }

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
            NextCategoryToPresent = -1;
        }

        private Round2Data() { }    // for Classify

        public Round2Data SelectCategory(int cat) { return this.ApplyToClone(r2d => { r2d.SelectedCategory = cat; }); }
        public Round2Data SelectQuestion(int index) { return this.ApplyToClone(r2d => { r2d.SelectedQuestion = index; }); }
        public Round2Data GiveAnswer(bool correct) { return this.ApplyToClone(r2d => { r2d.AnswerGiven = correct; }); }

        public Round2Data Pass()
        {
            return this.ApplyToClone(r2d =>
            {
                r2d.CurrentContestant = (CurrentContestant + 1) % Contestants.Length;
            });
        }

        public Round2Data DismissQuestion()
        {
            return this.ApplyToClone(r2d =>
            {
                r2d.QuestionsUsed = QuestionsUsed.ReplaceIndex(SelectedCategory.Value, qu => qu.ReplaceIndex(SelectedQuestion.Value, true));
                r2d.Contestants = Contestants.ReplaceIndex(CurrentContestant, c => c.IncScore((SelectedQuestion.Value + 1) * (AnswerGiven.Value ? 1 : -1)));
                r2d.SelectedCategory = null;
                r2d.SelectedQuestion = null;
                r2d.AnswerGiven = null;
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
                var totalRemainingPoints = QuestionsUsed.Sum(cat => cat.Select((taken, i) => taken ? 0 : i + 1).Sum());
                var contestantsSorted = Contestants.OrderByDescending(c => c.Score).ToArray();

                var tt = new TextTable { ColumnSpacing = 2 };
                var row = 0;

                const int colContSel = 0;
                const int colContName = colContSel + 1;
                const int colContScore = colContName + 1;
                const int colContRank = colContScore + 1;
                const int numColsCont = colContRank - colContSel;

                const int colCatSel = colContRank + 2;
                const int colCatName = colCatSel + 1;
                const int colCatQs = colCatName + 1;

                var qs = Ut.NewArray(
                    Tuple.Create("Very Easy", "VE"),
                    Tuple.Create("Easy", "Ea"),
                    Tuple.Create("Medium", "Me"),
                    Tuple.Create("Hard", "Ha"),
                    Tuple.Create("Very Hard", "VH"));

                var numColsCat = 1 + qs.Length;

                tt.SetCell(colContName, row, "CONTESTANTS".Color(ConsoleColor.White), alignment: HorizontalTextAlignment.Center, background: ConsoleColor.DarkGreen, colSpan: numColsCont);
                tt.SetCell(colCatName, row, "CATEGORIES".Color(ConsoleColor.White), alignment: HorizontalTextAlignment.Center, background: ConsoleColor.DarkGreen, colSpan: numColsCat);

                row++;

                tt.SetCell(colContName, row, "Contestant".Color(ConsoleColor.White));
                tt.SetCell(colContScore, row, "Score".Color(ConsoleColor.White), alignment: HorizontalTextAlignment.Right);
                tt.SetCell(colContRank, row, "Rk".Color(ConsoleColor.White), alignment: HorizontalTextAlignment.Right);

                tt.SetCell(colCatName, row, "Category".Color(ConsoleColor.White));
                for (int i = 0; i < qs.Length; i++)
                    tt.SetCell(colCatQs + i, row, qs[i].Item2.Color(ConsoleColor.White));
                tt.SetCell(colCatQs + qs.Length, row, " ");

                row++;

                var numThrough = 0;
                for (int i = 0; i < Math.Max(Contestants.Length, Categories.Length); i++)
                {
                    ConsoleColor? bg = null;
                    if (i < Contestants.Length)
                    {
                        ConsoleColoredString str = "";
                        if (Contestants[i].Score - totalRemainingPoints > contestantsSorted[6].Score)
                            str += "✓".Color(ConsoleColor.Green);
                        else if (Contestants[i].Score + totalRemainingPoints < contestantsSorted[5].Score)
                            str += "✗".Color(ConsoleColor.Red);
                        if (i == CurrentContestant)
                        {
                            str += "▶".Color(ConsoleColor.White);
                            bg = ConsoleColor.DarkBlue;
                        }
                        tt.SetCell(colContSel, row, str);
                        tt.SetCell(colContName, row, Contestants[i].Name.Color(ConsoleColor.Yellow), background: bg);
                        tt.SetCell(colContScore, row, Contestants[i].Score.ToString().Color(ConsoleColor.Cyan), alignment: HorizontalTextAlignment.Right, background: bg);
                        var rank = Contestants.Count(c => c.Score > Contestants[i].Score) + 1;
                        if (rank <= NumContestantsNeeded)
                            numThrough++;
                        tt.SetCell(colContRank, row, rank.ToString().Color(rank <= NumContestantsNeeded ? ConsoleColor.Magenta : ConsoleColor.DarkRed), alignment: HorizontalTextAlignment.Right, background: bg);
                    }

                    if (i < Categories.Length)
                    {
                        bg = null;
                        if (i == SelectedCategory)
                        {
                            tt.SetCell(colCatSel, row, "▶");
                            bg = ConsoleColor.DarkBlue;
                        }
                        var dark = NextCategoryToPresent != null && NextCategoryToPresent.Value <= i;
                        tt.SetCell(colCatName, row, Categories[i].Name.Color(dark ? ConsoleColor.DarkYellow : ConsoleColor.Yellow), background: bg);
                        for (int j = 0; j < qs.Length; j++)
                            tt.SetCell(colCatQs + j, row, j >= QuestionsUsed[i].Length ? "?".Color(dark ? ConsoleColor.DarkRed : ConsoleColor.Red) : QuestionsUsed[i][j] ? "" : "█".Color(dark ? ConsoleColor.DarkCyan : ConsoleColor.Cyan), background: bg);
                    }

                    row++;
                }
                tt.SetCell(colContRank, row, "{0}/{1}".Color(numThrough == NumContestantsNeeded ? ConsoleColor.White : ConsoleColor.DarkGray).Fmt(numThrough, NumContestantsNeeded), alignment: HorizontalTextAlignment.Right);
                row++;

                return "{0}\n\nTotal remaining points: {1/Magenta}".Color(ConsoleColor.White).Fmt(
                    tt.ToColoredString(),
                    totalRemainingPoints
                );
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void BeforeSerialize()
        {
        }

        public void AfterDeserialize()
        {
            if (QuestionsUsed.Length != QuizData.Round2Categories.Length)
                QuestionsUsed = QuizData.Round2Categories.Select((cat, ci) => cat.Questions.Select((q, qi) => ci >= QuestionsUsed.Length || qi >= QuestionsUsed[ci].Length ? false : QuestionsUsed[ci][qi]).ToArray()).ToArray();
        }
    }
}
