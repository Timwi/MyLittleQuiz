using System;
using System.Collections.Generic;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round4_Final_Congratulations : QuizStateBase
    {
        public Round4Data Data { get; private set; }

        public Round4_Final_Congratulations(Round4Data data)
        {
            Data = data;
        }

        private Round4_Final_Congratulations() { }    // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string JsMethod
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override object JsParameters
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}