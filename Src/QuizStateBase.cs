using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Servers;
using RT.TagSoup;
using RT.Util.Consoles;

namespace QuizGameEngine
{
    public abstract class QuizStateBase
    {
        public QuizStateBase PreviousState;
        public ConsoleKey? PreviousTransitionKey;

        public abstract TransitionPrompt[] Transitions { get; }
        public abstract ConsoleColoredString Describe { get; }

        public QuizStateBase(QuizStateBase prevState, ConsoleKey? prevTransitionKey)
        {
            PreviousState = prevState;
            PreviousTransitionKey = prevTransitionKey;
        }

        protected QuizStateBase() { }   // for Classify
    }
}
