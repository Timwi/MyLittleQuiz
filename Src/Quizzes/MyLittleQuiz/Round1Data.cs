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
        [ClassifyNotNull]
        public Dictionary<Difficulty, int[]> QuestionsTaken { get; private set; } = new Dictionary<Difficulty, int[]> { { Difficulty.Easy, new int[0] }, { Difficulty.Medium, new int[0] } };
        [ClassifyIgnoreIfDefault]
        public int? SelectedContestant { get; private set; }
        [ClassifyIgnoreIfDefault]
        public Difficulty? CurrentDifficulty { get; private set; }
        [ClassifyIgnoreIfDefault]
        public int? CurrentQuestionIndex { get; private set; }
        [ClassifyIgnoreIfDefault]
        public bool? AnswerGiven { get; private set; }

        public bool MusicStarted { get; private set; }
        public Round1Data StartMusic()
        {
            return this.ApplyToClone(c => { c.MusicStarted = true; });
        }

        public QuestionBase CurrentQuestion { get { return CurrentQuestionIndex.NullOr(cqi => CurrentDifficulty.NullOr(cd => Questions[cd][cqi])); } }

        public Round1Data(QuizData quizData, IEnumerable<Contestant> contestants)
        {
            QuizData = quizData;
            Contestants = contestants.Select(c => new Round1Contestant(c.Name, c.Roll)).ToArray();
            QuestionsTaken = new Dictionary<Difficulty, int[]> { { Difficulty.Easy, new int[0] }, { Difficulty.Medium, new int[0] } };
            SelectedContestant = null;
            CurrentDifficulty = null;
            AnswerGiven = null;
            MusicStarted = false;
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
                var numOut = Contestants.Count(c => c.IsOut);
                var numThrough = Contestants.Count(c => c.IsThrough);
                var numRemaining = Contestants.Count(c => c.IsStillInGame);

                return "{0/White}\n{1}\n{2}{3}\n\n{4/White}\n{5}{6}".Color(null).Fmt(
                    /* 0 */ "Contestants:",
                    /* 1 */ Contestants.SelectIndexWhere(c => c.IsStillInGame).Select(i =>
                                    (i == SelectedContestant ? "[".Color(ConsoleColor.Cyan, ConsoleColor.DarkBlue) : null) +
                                    "•".Color(ConsoleColor.White, ConsoleColor.DarkGreen).Repeat(Contestants[i].NumCorrect).JoinColoredString() +
                                    "•".Color(ConsoleColor.Black, ConsoleColor.Red).Repeat(Contestants[i].NumWrong).JoinColoredString() +
                                    (i + 1).ToString().Color(ConsoleColor.White, i == SelectedContestant ? ConsoleColor.DarkBlue : (ConsoleColor?) null) +
                                    (i == SelectedContestant ? "]".Color(ConsoleColor.Cyan, ConsoleColor.DarkBlue) : null))
                                    .JoinColoredString(" ")
                                    .WordWrap(ConsoleUtil.WrapToWidth(), 4)
                                    .JoinColoredString(Environment.NewLine),
                    /* 2 */ "{0/Red} out, {1/Green} through, {2/Cyan} remaining, need {3/Yellow} more through or {4/Magenta} more out".Color(null).Fmt(numOut, numThrough, numRemaining, QuizData.Round1NumContestantsNeeded - numThrough, numThrough + numRemaining - QuizData.Round1NumContestantsNeeded),
                    /* 3 */ SelectedContestant == null ? null : "\n\n{0/White}\n{1/Green} {2/Red} {3/Yellow}".Color(null).Fmt("Selected contestant:", Contestants[SelectedContestant.Value].NumCorrect, Contestants[SelectedContestant.Value].NumWrong, Contestants[SelectedContestant.Value].Name),
                    /* 4 */ "Number of questions available:",
                    /* 5 */ Questions.Select(kvp => "{0/Cyan}: {1/Magenta}".Color(null).Fmt(kvp.Key, kvp.Value.Length - QuestionsTaken[kvp.Key].Length)).JoinColoredString("\n"),
                    /* 6 */ CurrentQuestion == null ? null : "\n\n" + CurrentQuestion.Describe(AnswerGiven)
                );
            }
        }

        public Round1Data SelectContestant(int index)
        {
            return this.ApplyToClone(c =>
            {
                c.SelectedContestant = index;

                // Select a question to ask this contestant
                var difficulty = Contestants[index].NumCorrect > 0 ? Difficulty.Medium : Difficulty.Easy;
                if (difficulty == Difficulty.Easy && (!Questions.ContainsKey(difficulty) || Questions[difficulty].Length <= QuestionsTaken[difficulty].Length))
                    difficulty = Difficulty.Medium;
                c.CurrentDifficulty = difficulty;
                var availableQuestionIndexes = Enumerable.Range(0, Questions[difficulty].Length).Except(QuestionsTaken[difficulty]).ToArray();
                c.CurrentQuestionIndex = availableQuestionIndexes[Rnd.Next(availableQuestionIndexes.Length)];
            });
        }

        public Round1Data DisqualifySelectedContestant()
        {
            return this.ApplyToClone(c =>
            {
                c.Contestants = Contestants.ReplaceIndex(SelectedContestant.Value, ct => ct.Disqualify());
                c.SelectedContestant = null;
                c.CurrentQuestionIndex = null;
                c.CurrentDifficulty = null;
            });
        }

        public Round1Data GiveAnswer(bool correct)
        {
            return this.ApplyToClone(c => { c.AnswerGiven = correct; });
        }

        public Round1Data DismissQuestion()
        {
            return this.ApplyToClone(cl =>
            {
                // Take a copy of this dictionary before modifying it
                cl.QuestionsTaken = new Dictionary<Difficulty, int[]>(QuestionsTaken);
                cl.QuestionsTaken[CurrentDifficulty.Value] = cl.QuestionsTaken[CurrentDifficulty.Value].Concat(CurrentQuestionIndex.Value).ToArray();

                // Update the contestant’s score
                cl.Contestants = Contestants.ReplaceIndex(SelectedContestant.Value, c => c.IncScore(AnswerGiven.Value));

                cl.SelectedContestant = null;
                cl.CurrentDifficulty = null;
                cl.CurrentQuestionIndex = null;
                cl.AnswerGiven = null;
            });
        }
    }
}
