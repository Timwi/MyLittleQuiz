using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RT.Util.Consoles;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round3_SetPoker_Reveal : QuizStateBase
    {
        public Round3Data Data { get; private set; }

        public Round3_SetPoker_Reveal(Round3Data data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            Data = data;
        }

        private Round3_SetPoker_Reveal() { } // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.S, "Show teams", () => new Round3_SetPoker_ShowTeams(Data));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "Revealing set {0/Yellow}".Color(ConsoleColor.Green).Fmt(Data.QuizData.Round3Sets[Data.SetIndex - 1].Name);
            }
        }

        public override string JsMethod
        {
            get { return "r3_reveal"; }
        }

        public override object JsParameters
        {
            get
            {
                var set = Data.QuizData.Round3Sets[Data.SetIndex - 1];
                return new { set = set.Name, remaining = set.Answers.Except(Data.AnswersGiven).Order().ToArray() };
            }
        }
    }
}
