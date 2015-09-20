using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round4_Start : QuizStateBase
    {
        public Round4Data Data { get; private set; }

        public Round4_Start(Round4Data data)
        {
            Data = data;
        }

        private Round4_Start() { }  // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get { throw new NotImplementedException(); }
        }

        public override RT.Util.Consoles.ConsoleColoredString Describe
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
