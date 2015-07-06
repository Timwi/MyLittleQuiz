using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Servers;
using RT.TagSoup;
using RT.Util.Consoles;
using RT.Util.Serialization;

namespace QuizGameEngine
{
    public abstract class QuizStateBase
    {
        [ClassifyIgnoreIfDefault]
        public QuizStateBase PreviousState;
        [ClassifyIgnoreIfDefault]
        public ConsoleKey? PreviousTransitionKey;
        [ClassifyIgnoreIfDefault]
        public string UndoLine;

        public abstract Transition[] Transitions { get; }
        public abstract ConsoleColoredString Describe { get; }

        public QuizStateBase(QuizStateBase prevState, ConsoleKey? prevTransitionKey, string undoLine)
        {
            PreviousState = prevState;
            PreviousTransitionKey = prevTransitionKey;
            UndoLine = undoLine;
        }

        protected QuizStateBase() { }   // for Classify
    }
}
