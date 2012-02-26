using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Fusion.Net
{
    internal class StateObject
    {
        public Socket socket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
        public int TargetLength = 0;
        public string ID = "";
    }

    public delegate void RequestHandler(Request req, Response res);

    public class Server
    {
        private ManualResetEvent re = new ManualResetEvent(false);
        protected RequestHandler responseCallback;
        private int counter = 0;

        /// <summary>
        /// Create a new Fusion server
        /// </summary>
        /// <param name="callback">A callback to be invoked whenever data is recieved from the client</param>
        public Server(RequestHandler callback)
        {
            responseCallback = callback;
        }

        /// <summary>
        /// Begin listening
        /// </summary>
        public void Listen(int port)
        {
            Listen(IPAddress.Any, port);
        }

        /// <summary>
        /// Begin listening
        /// </summary>
        public void Listen(IPAddress ip, int port)
        {
            IPEndPoint endpoint = new IPEndPoint(ip, port);
            Socket listener = new Socket(endpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(endpoint);
            listener.Listen(port);

            // Start listening loop
            while (true)
            {
                re.Reset();
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                re.WaitOne();
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket socket = listener.EndAccept(ar);

            re.Set();

            StateObject state = new StateObject();
            state.socket = socket;

            counter++;
            state.ID = counter.ToString();

            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(Read), state);
        }

        internal virtual void Read(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;

            int read = state.socket.EndReceive(ar);
            if (read > 0)
            {
                string body = Encoding.ASCII.GetString(state.buffer, 0, read);
                Request req = new Request(state, body);
                Response res = new Response(state, req);
                responseCallback(req, res);
            }

            state.socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(Read), state);
        }
    }
}
