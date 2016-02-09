using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round4_Final_ShowContestants : QuizStateBase
    {
        public Round4Data Data { get; private set; }

        public Round4_Final_ShowContestants(Round4Data data)
        {
            Data = data;
        }

        private Round4_Final_ShowContestants() { }  // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.Q, "Ask {0} the next question".Fmt(Data.Contestants[Data.WhoseTurn].Name), () => new Round4_Final_Q(Data));
            }
        }

        public override ConsoleColoredString Describe { get { return Data.Describe; } }

        public override string JsMethod
        {
            get { return "r4_showContestants"; }
        }

        public override object JsParameters
        {
            get { return new { contestants = Data.Contestants, answers = Data.Answers }; }
        }
    }
}
