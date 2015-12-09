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

        public Round3_SetPoker_Play(Round3Data data, int bid, string[] answersGiven = null, bool[] answersCorrect = null)
            : base(data, answersGiven, answersCorrect)
        {
            Bid = bid;
        }

        private Round3_SetPoker_Play() { }  // for Classify

        public override Round3_SetPoker_PlayBase GiveAnswer(string answer, bool correct)
        {
            return new Round3_SetPoker_Play(Data, Bid, AnswersGiven.Concat(answer).ToArray(), AnswersCorrect.Concat(correct).ToArray());
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "Current set: {0/Yellow}\nAnswers so far:\n{1}\nGot {2/Cyan}/{3/Yellow}".Color(ConsoleColor.White).Fmt(
                    /* {0} */ CurrentSet.Name,
                    /* {1} */ AnswersGiven.Select((a, i) => a.Color(AnswersCorrect[i] ? ConsoleColor.Green : ConsoleColor.Red) + "\n").JoinColoredString(),
                    /* {2} */ AnswersCorrect.Count(c => c),
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
            get { return new { answers = CorrectAnswersGiven.ToArray(), remaining = Bid - CorrectAnswersGiven.Count() }; }
        }
    }
}
