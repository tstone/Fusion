using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Fusion.Net;

namespace Fusion.Http
{
    public delegate void HttpRequestHandler(HttpRequest req, HttpResponse res);
    
    public class HttpServer : Server
    {
        protected HttpRequestHandler httpResponseCallback;
        
        public HttpServer(HttpRequestHandler callback) : base(null)
        {
            httpResponseCallback = callback;
        }
        
        internal override void Read(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            int read = 0;

            try { read = state.socket.EndReceive(ar); }
            catch (ObjectDisposedException ode) { return; }

            if (read > 0)
            {
                // Accumulate response...
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read));
                string body = state.sb.ToString();
                
                // ...until we hit the end of the request
                int loc = body.IndexOf("\r\n\r\n");
                if (loc > -1)
                {
                    // Check if there is a Content-Length
                    // If so, make sure we've got the entire body 
                    // before invoking the response callback
                    string topHalf = body.Substring(0, loc);
                    int cl = topHalf.IndexOf("Content-Length: ");
                    if (cl > -1)
                    {
                        string val = topHalf.Substring(cl + 16);
                        val = val.Substring(0, val.IndexOf("\n")).Trim();
                        state.TargetLength = Convert.ToInt32(val);

                        // Check the length
                        string bottomHalf = body.Substring(loc).Trim();
                        if (bottomHalf.Length == state.TargetLength)
                        {
                            HttpRequest req = new HttpRequest(state, topHalf, bottomHalf);
                            HttpResponse res = new HttpResponse(state, req);
                            httpResponseCallback(req, res);
                            state.sb.Clear();
                        }
                    }
                    // If there's no Content-Length then the \r\n\r\n
                    // signifies the end of file
                    else
                    {
                        HttpRequest req = new HttpRequest(state, body, string.Empty);
                        HttpResponse res = new HttpResponse(state, req);
                        httpResponseCallback(req, res);
                        state.sb.Clear();
                    }
                }
            }

            try
            {
                state.socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(Read), state);
            }
            catch { }
        }
    }
}
