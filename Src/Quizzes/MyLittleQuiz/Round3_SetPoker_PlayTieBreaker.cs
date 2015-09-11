using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round3_SetPoker_PlayTieBreaker : QuizStateBase
    {
        public Round3Data Data { get; private set; }

        public Round3_SetPoker_PlayTieBreaker(Round3Data data)
        {
            Data = data;
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
