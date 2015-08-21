using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round2_Categories_Start : QuizStateBase
    {
        public Round2Data Data { get; private set; }

        public Round2_Categories_Start(string undoLine, Round2Data data) : base(undoLine) { Data = data; }
        private Round2_Categories_Start() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get { throw new NotImplementedException(); }
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
