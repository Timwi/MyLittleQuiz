using System;
using System.Collections.Generic;
using System.Linq;
using RT.Util.Consoles;
using RT.Util.Dialogs;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public class Round3_SetPoker_Play : Round3_SetPoker_PlayBase
    {
        public int Bid { get; private set; }

        public Round3_SetPoker_Play(Round3Data data, int bid, string[] answersGiven = null)
            : base(data, answersGiven)
        {
            Bid = bid;
        }

        private Round3_SetPoker_Play() { }  // for Classify

        public override Round3_SetPoker_PlayBase GiveAnswer(string answer)
        {
            return new Round3_SetPoker_Play(Data, Bid, AnswersGiven.Concat(answer).ToArray());
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "Current set: {0/Yellow}\nCorrect answers so far:\n{1}\nGot {2/Cyan}/{3/Yellow}".Color(ConsoleColor.White).Fmt(
                    /* {0} */ CurrentSet.Name,
                    /* {1} */ AnswersGiven.Select((a, i) => a.Color(ConsoleColor.Green) + "\n").JoinColoredString(),
                    /* {2} */ AnswersGiven.Length,
                    /* {3} */ Bid
                );
            }
        }

        public override string JsMethod
        {
            get { return "r3_play"; }
        }

        public override object JsParameters
        {
            get { return new { tie = false, answers = AnswersGiven, bid = Bid, remaining = Bid - AnswersGiven.Length }; }
        }
    }
}
