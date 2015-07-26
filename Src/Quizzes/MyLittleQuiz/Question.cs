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
    public abstract class QuestionBase
    {
        public string QuestionText;
    }

    public sealed class SimpleQuestion : QuestionBase
    {
        public string Answer;
    }

    public sealed class OneOfQuestion : QuestionBase
    {
        public string[] Answers;
    }
}
