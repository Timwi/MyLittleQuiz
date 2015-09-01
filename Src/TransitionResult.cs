using System;
using RT.Util.ExtensionMethods;
using RT.Util;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Util.Json;
using RT.Util.Serialization;

namespace QuizGameEngine
{
    public sealed class TransitionResult
    {
        public QuizStateBase State { get; private set; }
        public string UndoLine { get; private set; }
        public string JsMethod { get; private set; }
        public object JsParameters { get; private set; }

        public TransitionResult(QuizStateBase state, string undoLine = null, string jsMethod = null, object jsParams = null)
        {
            State = state;
            UndoLine = undoLine;
            JsMethod = jsMethod;
            JsParameters = jsParams;
        }
    }
}
