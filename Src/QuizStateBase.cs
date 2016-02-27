using System;
using System.Collections.Generic;
using RT.Util.Consoles;

namespace QuizGameEngine
{
    public abstract class QuizStateBase : ICloneable
    {
        public abstract IEnumerable<Transition> Transitions { get; }
        public abstract ConsoleColoredString Describe { get; }
        public abstract string JsMethod { get; }
        public abstract object JsParameters { get; }
        public virtual string JsMusic { get { return null; } }

        public QuizStateBase() { }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
