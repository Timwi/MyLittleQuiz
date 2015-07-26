using System;
using RT.Util;
using RT.Util.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Servers;
using RT.Util.Json;
using RT.Util.Serialization;

namespace QuizGameEngine
{
    sealed class QuizWebSocket : WebSocket
    {
        protected override void onBeginConnection()
        {
            base.onBeginConnection();
            lock (Program.Sockets)
                Program.Sockets.Add(this);
        }

        protected override void onEndConnection()
        {
            lock (Program.Sockets)
                Program.Sockets.Remove(this);
            base.onEndConnection();
        }

        protected override void onTextMessageReceived(string msg)
        {
            if (msg == "ping")
            {
                // Because of concurrency, make sure we access Program.Quiz.CurrentState only once.
                var state = Program.Quiz.CurrentState;
                if (state.JsMethod != null)
                    SendMessage(new JsonDict { { "method", state.JsMethod }, { "params", ClassifyJson.Serialize(state.JsParameters) } });
            }
            base.onTextMessageReceived(msg);
        }
    }
}
