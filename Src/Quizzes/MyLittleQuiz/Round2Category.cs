using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round2Category
    {
        public string Name { get; private set; }
        [ClassifyNotNull]
        public QuestionBase[] Questions { get; private set; }

        public Round2Category(string name, QuestionBase[] questions, bool[] questionTaken = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (questions == null)
                throw new ArgumentNullException("questions");

            Name = name;
            Questions = questions;
        }

        private Round2Category()    // for Classify
        {
            Questions = new QuestionBase[0];
        }
    }
}
