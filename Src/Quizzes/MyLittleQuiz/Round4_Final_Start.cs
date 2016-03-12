using System;
using System.Collections.Generic;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round4_Final_Start : QuizStateBase
    {
        public Round4Data Data { get; private set; }
        public bool PlayIntro { get; private set; }

        public Round4_Final_Start(Round4Data data, bool playIntro = false) { Data = data; PlayIntro = playIntro; }
        private Round4_Final_Start() { }    // for Classify

        public override ConsoleColoredString Describe { get { return "Start of Round 4 (Final)."; } }
        public override string JsMethod { get { return PlayIntro ? "r4_intro" : "blank"; } }
        public override object JsParameters { get { return null; } }
        public override string JsJingle { get { return PlayIntro ? Jingle.Round4Start.ToString() : null; } }
        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.I, "Intro Round 4", () => new Round4_Final_Start(Data, true));
                yield return Transition.Simple(ConsoleKey.S, "Show contestants", () => new Round4_Final_ShowContestants(Data));
            }
        }
    }
}
