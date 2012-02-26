using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusion.Net
{
    public class Request
    {
        private StateObject NetState;
        
        internal Request(StateObject state, string body)
        {
            this.NetState = state;
            this.Body = body;
        }

        //
        //  Members
        //

        public string Body { get; internal set; }

        //
        //  Methods
        //

        /* public void End()
        {
            NetState.socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
            NetState.socket.Close();
        } */
    }
}
