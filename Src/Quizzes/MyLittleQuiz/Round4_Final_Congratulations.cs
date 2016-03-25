using System;
using System.Collections.Generic;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round4_Final_Congratulations : QuizStateBase
    {
        public string WinnerName { get; private set; }

        public Round4_Final_Congratulations(string winnerName)
        {
            WinnerName = winnerName;
        }

        private Round4_Final_Congratulations() { }    // for Classify

        public override IEnumerable<Transition> Transitions { get { yield break; } }
        public override ConsoleColoredString Describe { get { return "{0/White} wins. Congratulations!".Color(ConsoleColor.Magenta).Fmt(WinnerName); } }

        public override string JsMethod { get { return "congratulations"; } }
        public override object JsParameters { get { return new { winner = WinnerName }; } }
        public override string JsMusic { get { return null; } }
        public override string JsJingle { get { return Jingle.WinnerAndOutro.ToString(); } }
    }
}