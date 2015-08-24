using System;
using System.Linq;
using RT.Util.Consoles;
using RT.Util.ExtensionMethods;
using RT.Util.Serialization;

namespace QuizGameEngine.Quizzes.SimpleQuiz
{
    public abstract class StateBase : QuizStateBase
    {
        [ClassifyNotNull, ClassifyIgnoreIfDefault, ClassifyIgnoreIfEmpty]
        public Contestant[] Contestants = new Contestant[0];
        [ClassifyNotNull, ClassifyIgnoreIfDefault, ClassifyIgnoreIfEmpty]
        public Tuple<string, string>[] Questions = new Tuple<string, string>[0];

        protected StateBase(Tuple<string, string>[] questions, Contestant[] contestants)
        {
            if (questions == null)
                throw new ArgumentNullException("questions");
            Questions = questions;
            Contestants = contestants ?? new Contestant[0];
        }

        protected StateBase() { }  // for Classify

        public override ConsoleColoredString Describe { get { return DescribeSel(null); } }

        public ConsoleColoredString DescribeSel(int? selectedContestant)
        {
            return "{0/White} ({1/White})\n{2}\n\n{3/Cyan} questions in the DB.".Color(null).Fmt(
                /* 0 */ "Contestants:",
                /* 1 */ Contestants.Length,
                /* 2 */ Contestants.Select((cn, i) => cn.ToString().Color(null, i == selectedContestant ? ConsoleColor.DarkBlue : (ConsoleColor?) null)).JoinColoredString("\n"),
                /* 3 */ Questions.Length);
        }
    }
}
