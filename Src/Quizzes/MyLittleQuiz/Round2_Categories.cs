using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round2_Categories : QuizStateBase
    {
        public Round2Data Data { get; private set; }

        public Round2_Categories(Round2Data data) { Data = data; }
        private Round2_Categories() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield break;
            }
        }

        public override ConsoleColoredString Describe
        {
            get { throw new NotImplementedException(); }
        }

        public override string JsMethod
        {
            get { throw new NotImplementedException(); }
        }

        public override object JsParameters
        {
            get { throw new NotImplementedException(); }
        }
    }
}
