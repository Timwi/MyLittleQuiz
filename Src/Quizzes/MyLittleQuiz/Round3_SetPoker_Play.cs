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

        public Round3_SetPoker_Play(Round3Data data, int bid)
            : base(data)
        {
            Bid = bid;
        }

        private Round3_SetPoker_Play() { }  // for Classify

        public override Round3_SetPoker_PlayBase GiveCorrectAnswer(string answer)
        {
            return new Round3_SetPoker_Play(Data.GiveCorrectAnswer(answer), Bid);
        }

        public override Round3_SetPoker_PlayBase GiveWrongAnswer()
        {
            return new Round3_SetPoker_Play(Data.GiveWrongAnswer(false), Bid);
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0}\n\nCorrect answers so far:\n{1}\nStrikes: {4/Red}\n\nGot {2/Cyan}/{3/Yellow}".Color(ConsoleColor.White).Fmt(
                    /* {0} */ base.Describe,
                    /* {1} */ Data.AnswersGiven.Select((a, i) => a.Color(ConsoleColor.Green) + "\n").JoinColoredString(),
                    /* {2} */ Data.AnswersGiven.Length,
                    /* {3} */ Bid,
                    /* {4} */ Data.WrongAnswers
                );
            }
        }

        public override string JsMethod
        {
            get { return "r3_play"; }
        }

        public override object JsParameters
        {
            get { return new { tie = false, answers = Data.AnswersGiven, bid = Bid, remaining = Bid - Data.AnswersGiven.Length, strikes = Data.WrongAnswers }; }
        }
    }
}
