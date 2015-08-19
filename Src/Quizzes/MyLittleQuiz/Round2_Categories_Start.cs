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
        public Round2Contestant[] Contestants { get; private set; }
        public QuizData Data { get; private set; }

        public Round2_Categories_Start(Round1Contestant[] contestants, QuizData data)
        {
            Contestants = contestants.Select(r1c => new Round2Contestant(r1c.Name, 0)).ToArray();
            Data = data;
        }

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
