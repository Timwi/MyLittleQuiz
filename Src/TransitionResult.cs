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
        public string JsMethod { get; private set; }
        public JsonValue JsParameters { get; private set; }

        public TransitionResult(QuizStateBase state, string jsMethod = null, object jsParams = null)
        {
            State = state;
            JsMethod = jsMethod;
            JsParameters = jsParams == null ? null : ClassifyJson.Serialize(jsParams);
        }

        public static implicit operator TransitionResult(QuizStateBase state) { return state == null ? null : new TransitionResult(state, state.JsMethod, state.JsParameters); }
    }
}
