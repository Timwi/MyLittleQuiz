using System;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round2Data : ICloneable
    {
        public QuizData QuizData { get; private set; }
        public Round2Contestant[] Contestants { get; private set; }
        public int CurrentContestant { get; private set; }
        public bool[][] QuestionsUsed { get; private set; }
        public object AnswerObject { get; private set; }

        [ClassifyIgnoreIfDefault]
        public int? SelectedCategory { get; private set; }
        [ClassifyIgnoreIfDefault]
        public int? SelectedQuestion { get; private set; }

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
        }

        private Round2Data() { }    // for Classify

        public Round2Data SelectCategory(int cat) { return this.ApplyToClone(r2d => { r2d.SelectedCategory = cat; }); }
        public Round2Data SelectQuestion(int index) { return this.ApplyToClone(r2d => { r2d.SelectedQuestion = index; }); }
        public Round2Data GiveAnswer(object answer) { return this.ApplyToClone(r2d => { r2d.AnswerObject = answer; }); }

        public Round2Data DismissQuestion()
        {
            return this.ApplyToClone(r2d =>
            {
                r2d.QuestionsUsed = QuestionsUsed.ReplaceIndex(SelectedCategory.Value, qu => qu.ReplaceIndex(SelectedQuestion.Value, false));
                r2d.Contestants = Contestants.ReplaceIndex(CurrentContestant, c => c.IncScore((SelectedQuestion.Value + 1) * (AnswerObject.Equals(false) ? -1 : 1)));
                r2d.SelectedCategory = null;
                r2d.SelectedQuestion = null;
                r2d.AnswerObject = null;
                r2d.CurrentContestant = (CurrentContestant + 1) % Contestants.Length;
            });
        }

        public ConsoleColoredString Describe
        {
            get
            {
                return Contestants
                    .Select(c => "{0/Yellow} (Score={1/Cyan})".Color(ConsoleColor.DarkCyan).Fmt(c.Name, c.Score))
                    .JoinColoredString(Environment.NewLine);
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
