﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Text;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round2_Categories_ShowContestants : QuizStateBase
    {
        public bool NoScores { get; private set; }
        public Round2Data Data { get; private set; }

        public Round2_Categories_ShowContestants(Round2Data data, bool noScores = false)
        {
            NoScores = noScores;
            Data = data;
        }
        private Round2_Categories_ShowContestants() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.I, "Intro", () =>
                {
                    // TODO
                });

                if (NoScores)
                    yield return Transition.Simple(ConsoleKey.S, "Show scores", () => new Round2_Categories_ShowContestants(Data));
                else
                    yield return Transition.Simple(ConsoleKey.C, "Show categories", () => new Round2_Categories_ShowCategories(Data));
            }
        }

        public override ConsoleColoredString Describe { get { return Data.DescribeContestants; } }
        public override string JsMethod { get { return "r2_showContestants"; } }
        public override object JsParameters
        {
            get
            {
                return new
                {
                    contestants = Data.Contestants,
                    current = Data.CurrentContestant,
                    noscores = NoScores
                };
            }
        }
    }
}
