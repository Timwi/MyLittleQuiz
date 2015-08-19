using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round2_Categories : QuizStateBase
    {
        public Round1Contestant[] Contestants { get; private set; }

        public Round2_Categories(Round1Contestant[] contestants)
        {
            Contestants = contestants;
        }

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
