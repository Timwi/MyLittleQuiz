using System;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Servers;
using RT.TagSoup;
using RT.Util;
using RT.Util.Consoles;
using RT.Util.Json;
using RT.Util.Serialization;

namespace QuizGameEngine
{
    public abstract class QuizStateBase : ICloneable
    {
        public abstract IEnumerable<Transition> Transitions { get; }
        public abstract ConsoleColoredString Describe { get; }
        public abstract string JsMethod { get; }
        public abstract object JsParameters { get; }

        public QuizStateBase() { }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
