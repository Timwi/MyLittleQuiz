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
    public sealed class Round2Category
    {
        public string Name { get; private set; }
        public QuestionBase[] Questions { get; private set; }
        public bool[] QuestionTaken { get; private set; }

        public Round2Category(string name, QuestionBase[] questions, bool[] questionTaken = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (questions == null)
                throw new ArgumentNullException("questions");

            Name = name;
            Questions = questions;
            QuestionTaken = questionTaken ?? new bool[questions.Length];
        }

        public Round2Category TakeQuestion(int index)
        {
            return new Round2Category(Name, Questions, QuestionTaken.ReplaceIndex(index, false));
        }
    }
}
