using System;
using System.Collections.Generic;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public sealed class Round4_Final_ShowContestants : QuizStateBase
    {
        public Round4Data Data { get; private set; }

        public Round4_Final_ShowContestants(Round4Data data)
        {
            Data = data;
        }

        private Round4_Final_ShowContestants() { }  // for Classify

        public override IEnumerable<Transition> Transitions
        {
            get
            {
                yield return Transition.Simple(ConsoleKey.Q, "Ask {0} the next question".Fmt(Data.Contestants[Data.WhoseTurn].Name), () => new Round4_Final_Q(Data));
            }
        }

        public override ConsoleColoredString Describe { get { return Data.Describe; } }

        public override string JsMethod { get { return "r4_showContestants"; } }
        public override string JsMusic { get { return @"/files/MyLittleQuiz/Music4.ogg"; } }
        public override object JsParameters
        {
            get { return new { contestants = Data.Contestants, answers = Data.Answers, minAnswers = Data.QuizData.Round4MinQuestions }; }
        }
    }
}
