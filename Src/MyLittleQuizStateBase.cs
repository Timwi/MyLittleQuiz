using System;
using System.Collections.Generic;

namespace Trophy.MyLittleQuiz
{
    public abstract class MyLittleQuizStateBase : QuizStateBase
    {
        public abstract QuestionBase CurrentQuestion { get; }
        public abstract TransitionResult GiveAnswer(bool correct);

        protected MyLittleQuizStateBase() { }   // for Classify

        protected IEnumerable<Transition> getAnswerTransitions()
        {
            yield return Transition.Simple(ConsoleKey.G, "Give correct answer", () => GiveAnswer(true));
            yield return Transition.Simple(ConsoleKey.Z, "Give wrong answer", () => GiveAnswer(false));
        }

        protected abstract string Round { get; }
    }
}
