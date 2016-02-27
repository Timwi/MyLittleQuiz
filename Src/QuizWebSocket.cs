using System.Net;
using RT.Servers;
using RT.Util.ExtensionMethods;
using RT.Util.Json;
using RT.Util.Serialization;

namespace QuizGameEngine
{
    sealed class QuizWebSocket : WebSocket
    {
        private IPEndPoint _endpoint;

        public QuizWebSocket(IPEndPoint endpoint)
        {
            _endpoint = endpoint;
        }

        protected override void onBeginConnection()
        {
            base.onBeginConnection();
            lock (Program.Sockets)
                Program.Sockets.Add(this);
            Program.LogMessage("WebSocket connection from {0}".Fmt(_endpoint));
        }

        protected override void onEndConnection()
        {
            lock (Program.Sockets)
                Program.Sockets.Remove(this);
            base.onEndConnection();
            Program.LogMessage("CLOSED WebSocket connection from {0}".Fmt(_endpoint));
        }

        protected override void onTextMessageReceived(string msg)
        {
            if (msg == "ping")
            {
                // Because of concurrency, make sure we access Program.Quiz.CurrentState only once.
                var state = Program.Quiz.CurrentState;
                if (state.JsMethod != null)
                {
                    var prms = ClassifyJson.Serialize(state.JsParameters);
                    if (prms.ContainsKey(":fulltype"))
                        prms.Remove(":fulltype");
                    SendLoggedMessage(new JsonDict { { "method", state.JsMethod }, { "params", prms }, { "music", state.JsMusic } });
                }
            }
            base.onTextMessageReceived(msg);
        }

        public void SendLoggedMessage(JsonValue json)
        {
            Program.LogMessage("{0}: {1}".Fmt(_endpoint, JsonValue.ToString(json)));
            SendMessage(json);
        }
    }
}
