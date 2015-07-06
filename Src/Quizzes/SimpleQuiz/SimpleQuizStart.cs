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

        public SimpleQuizStart(QuizStateBase prevState, ConsoleKey? prevTransitionKey, string undoLine, Tuple<string, string>[] questions, string[] contestants = null)
            : base(prevState, prevTransitionKey, undoLine)
        {
            if (questions == null)
                throw new ArgumentNullException("questions");
            Questions = questions;
            ContestantNames = contestants ?? new string[0];
        }

        private SimpleQuizStart() { }   // for Classify

        public override Transition[] Transitions
        {
            get
            {
                return Ut.NewArray(
                    Transition.String(ConsoleKey.N, "New contestant", "Contestant name: ", name => new SimpleQuizStart(this, ConsoleKey.N, "Create contestant: {0}".Fmt(name), Questions, ContestantNames.Concat(name).ToArray()), true),
                    Transition.Select(ConsoleKey.D, "Delete contestant", ContestantNames, index => new SimpleQuizStart(this, ConsoleKey.D, "Delete contestant: {0}".Fmt(ContestantNames[index]), Questions, ContestantNames.RemoveIndex(index)))
                );
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
