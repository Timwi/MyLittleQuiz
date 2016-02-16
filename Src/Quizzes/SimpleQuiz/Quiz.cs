using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizGameEngine.Quizzes.SimpleQuiz
{
    public sealed class Quiz : QuizBase
    {
        public override string CssJsFilename { get { return "SimpleQuiz"; } }

        public Quiz(Tuple<string, string>[] questions)
        {
            _currentState = new StateSetup(questions);
        }
        private Quiz() { }    // for Classify
    }
}
