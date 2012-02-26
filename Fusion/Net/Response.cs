using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion;

namespace Fusion.Net
{
    public class Response
    {
        protected byte[] Buffer;
        protected bool Ended = false;
        internal StateObject NetState;

        public Request Request { get; protected set; }

        public string ID
        {
            get { return NetState.ID; }
        }

        internal Response(StateObject state, Request req)
        {
            this.Buffer = new byte[0];
            this.NetState = state;
            this.Request = req;
        }

        //
        //  Methods
        
        public virtual void End()
        {
            this.Stream(Buffer);
            Buffer = new byte[0];
            Ended = true;
        }

        public virtual void Stream(string data) { Stream(Encoding.ASCII.GetBytes(data)); }
        public virtual void Stream(byte[] data)
        {
            try { NetState.socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(StreamCallback), NetState); }
            catch (ObjectDisposedException ode) { }
        }

        public virtual void Write(string data) { Write(Encoding.ASCII.GetBytes(data)); }
        public virtual void Write(byte[] data)
        {
            Buffer = Helpers.ConcatBytes(Buffer, data);
        }

        //
        // Helpers

        private void StreamCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            try
            {
                int bytes = state.socket.EndSend(ar);
                if (bytes == 0 && Ended)
                {
                    state.socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    state.socket.Close();
                }
            }
            catch { }
        }
    }
}
