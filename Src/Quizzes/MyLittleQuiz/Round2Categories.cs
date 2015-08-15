using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round2Categories : QuizStateBase
    {
        public Contestant[] Contestants { get; private set; }

        public Round2Categories(Contestant[] contestants)
        {
            Contestants = contestants;
        }

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
