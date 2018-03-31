﻿using System;
using System.Collections.Generic;
using RT.Util.Consoles;

namespace Trophy.MyLittleQuiz
{
    public sealed class Round1_Elimination_Beginning : QuizStateBase
    {
        public Round1Data Data { get; private set; }
        public Round1_Elimination_Beginning(Round1Data data) { Data = data; }
        private Round1_Elimination_Beginning() { }     // for Classify        

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.I, "Intro Round 1", "r1_intro", jsJingle: Jingle.Round1Start.ToString());
                yield return Transition.Simple(ConsoleKey.S, "Start Round: Elimination Round", () => new Round1_Elimination_ShowContestants(Data));
            }
        }

        public override ConsoleColoredString Describe { get { return ""; } }
        public override string JsMethod { get { return "blank"; } }
        public override string JsJingle { get { return null; } }
        public override object JsParameters { get { return new { bgclass = "r1" }; } }
    }
}