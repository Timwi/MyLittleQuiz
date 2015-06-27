using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util.Consoles;
using RT.Util.Serialization;
using RT.Util;

namespace QuizGameEngine.Quizzes.SimpleQuiz
{
    public class SimpleQuizStart : QuizStateBase
    {
        [ClassifyNotNull, ClassifyIgnoreIfDefault, ClassifyIgnoreIfEmpty]
        public string[] ContestantNames = new string[0];
        [ClassifyNotNull, ClassifyIgnoreIfDefault, ClassifyIgnoreIfEmpty]
        public Tuple<string, string>[] Questions = new Tuple<string, string>[0];

        public SimpleQuizStart(QuizStateBase prevState, ConsoleKey? prevTransitionKey, Tuple<string, string>[] questions, string[] contestants = null)
            : base(prevState, prevTransitionKey)
        {
            if (questions == null)
                throw new ArgumentNullException("questions");
            Questions = questions;
            ContestantNames = contestants ?? new string[0];
        }

        private SimpleQuizStart() { }   // for Classify

        public override TransitionPrompt[] Transitions
        {
            get
            {
                return Ut.NewArray(
                    new TransitionPrompt(ConsoleKey.N, "New contestant", "Contestant name: ", name => new SimpleQuizStart(this, ConsoleKey.N, Questions, ContestantNames.Concat(name).ToArray()), true));
            }
        }

        public override ConsoleColoredString Describe
        {
            get
            {
                return "{0/White}\n{1}\n\n{2/Cyan} questions in the DB.".Color(null).Fmt(
                    /* 0 */ "Contestants:",
                    /* 1 */ ContestantNames.JoinString("\n"),
                    /* 2 */ Questions.Length);
            }
        }
    }
}
