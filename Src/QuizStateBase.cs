using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Servers;
using RT.TagSoup;
using RT.Util.Consoles;
using RT.Util.Json;
using RT.Util.Serialization;

namespace QuizGameEngine
{
    public abstract class QuizStateBase
    {
        [ClassifyIgnoreIfDefault]
        public string UndoLine;

        public abstract Transition[] Transitions { get; }
        public abstract ConsoleColoredString Describe { get; }

        public QuizStateBase(string undoLine)
        {
            UndoLine = undoLine;
        }

        protected QuizStateBase() { }   // for Classify
    }
}
