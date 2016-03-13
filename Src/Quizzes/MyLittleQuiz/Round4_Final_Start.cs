using System;
using System.Collections.Generic;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round4_Final_Start : QuizStateBase
    {
        public Round4Data Data { get; private set; }

        public Round4_Final_Start(Round4Data data) { Data = data; }
        private Round4_Final_Start() { }    // for Classify

        public override ConsoleColoredString Describe { get { return "Start of Round 4 (Final)."; } }
        public override string JsMethod { get { return "blank"; } }
        public override object JsParameters { get { return null; } }
        public override string JsJingle { get { return null; } }
        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.I, "Intro Round 4", "r4_intro", jsJingle: Jingle.Round4Start.ToString());
                yield return Transition.Simple(ConsoleKey.S, "Show contestants", () => new Round4_Final_ShowContestants(Data));
            }
        }
    }
}
