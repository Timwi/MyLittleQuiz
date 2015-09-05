using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util;
using RT.Util.ExtensionMethods;

namespace QuizGameEngine.Quizzes.MyLittleQuiz
{
    public abstract class MyLittleQuizStateBase : QuizStateBase
    {
        public abstract QuestionBase CurrentQuestion { get; }
        public abstract QuizStateBase GiveAnswer(object answer);

        protected MyLittleQuizStateBase() { }   // for Classify

        protected IEnumerable<Transition> getAnswerTransitions()
        {
            return CurrentQuestion.CorrectAnswerInfos
                .Concat(Tuple.Create(ConsoleKey.Z, "Wrong", (object) false))
                .Select(answerInfo => Transition.Simple(
                    answerInfo.Item1,
                    "Answer: " + answerInfo.Item2,
                    () => GiveAnswer(answerInfo.Item3).With("showA", new { answer = answerInfo.Item3 })));
        }
    }
}
