using System;
using System.Linq;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using RT.Util.Consoles;
using RT.Util.Dialogs;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round3_SetPoker_PlayTieBreaker : Round3_SetPoker_PlayBase
    {
        public bool TeamAsTurn { get; private set; }
        public bool TeamACorrect { get; private set; }

        public Round3_SetPoker_PlayTieBreaker(Round3Data data, string[] answersGiven = null, bool[] answersCorrect = null, bool teamAsTurn = true, bool teamACorrect = false)
            : base(data, answersGiven, answersCorrect)
        {
            TeamAsTurn = teamAsTurn;
            TeamACorrect = teamACorrect;
        }

        private Round3_SetPoker_PlayTieBreaker() { }    // for Classify

        public override Round3_SetPoker_PlayBase GiveAnswer(string answer, bool correct)
        {
            return new Round3_SetPoker_PlayTieBreaker(Data, AnswersGiven.Concat(answer).ToArray(), AnswersCorrect.Concat(correct).ToArray(), !TeamAsTurn, correct);
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}’s turn\n{1}".Color(null).Fmt(
                    /* 0 */ TeamAsTurn ? "Team A" : "Team B",
                    /* 1 */ TeamAsTurn ? null : "Team A’s last answer was ".Color(ConsoleColor.Cyan) + (TeamACorrect ? "correct".Color(ConsoleColor.Green) : "wrong".Color(ConsoleColor.Red))
                );
            }
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
