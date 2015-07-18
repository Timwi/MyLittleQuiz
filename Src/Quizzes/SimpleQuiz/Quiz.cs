using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGameEngine.Quizzes.SimpleQuiz
{
    public sealed class Quiz : QuizBase
    {
        public override byte[] Css { get { return Resources.SimpleQuizCss; } }
        public override byte[] Js { get { return Resources.SimpleQuizJs; } }
        public override string CssJsFilename { get { return "SimpleQuiz"; } }

        public Quiz(Tuple<string, string>[] questions)
        {
            _currentState = new StateSetup(null, questions);
        }
        private Quiz() { }    // for Classify
    }
}
