using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round2Data : ICloneable
    {
        public QuizData Data { get; private set; }
        public Round2Contestant[] Contestants { get; private set; }
        public int CurrentContestant { get; private set; }
        public bool[][] QuestionsUsed { get; private set; }
        public int? SelectedCategory { get; private set; }
        public int? SelectedQuestion { get; private set; }

        public Round2Data(QuizData data, Round2Contestant[] contestants)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (contestants == null)
                throw new ArgumentNullException("contestants");

            Data = data;
            Contestants = contestants;
            QuestionsUsed = data.Round2Questions.Select(cat => cat.Questions.Select(q => false).ToArray()).ToArray();
            SelectedCategory = null;
            SelectedQuestion = null;
            CurrentContestant = 0;
        }

        private Round2Data() { }    // for Classify

        public Round2Data DismissQuestion()
        {
            return this.ApplyToClone(r2d =>
            {
                r2d.QuestionsUsed = QuestionsUsed.ReplaceIndex(SelectedCategory.Value, qu => qu.ReplaceIndex(SelectedQuestion.Value, false));
                r2d.SelectedCategory = null;
                r2d.SelectedQuestion = null;
                r2d.CurrentContestant = (CurrentContestant + 1) % Contestants.Length;
            });
        }

        public Round2Data SelectCategory(int cat) { return this.ApplyToClone(r2d => { r2d.SelectedCategory = cat; }); }
        public Round2Data SelectQuestion(int index) { return this.ApplyToClone(r2d => { r2d.SelectedQuestion = index; }); }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
