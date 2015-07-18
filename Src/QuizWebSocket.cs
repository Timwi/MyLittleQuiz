using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RT.Servers;

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
    }
}
