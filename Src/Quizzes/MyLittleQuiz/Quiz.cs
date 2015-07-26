using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Quiz : QuizBase
    {
        public override byte[] Css { get { return Resources.MyLittleQuizCss; } }
        public override byte[] Js { get { return Resources.MyLittleQuizJs; } }
        public override string CssJsFilename { get { return "MyLittleQuiz"; } }

        public Quiz()
        {
            _currentState = new Setup();
        }
    }
}
