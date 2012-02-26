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
            NetState.socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(StreamCallback), NetState);
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
            int bytes = state.socket.EndSend(ar);
            //Console.WriteLine(" ** (" + state.ID + ") StreamCallback + Bytes:" + bytes.ToString());

            if (bytes == 0 && Ended)
            {
                state.socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                state.socket.Close();                
            }
        }
    }
}
