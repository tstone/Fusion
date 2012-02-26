using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Fusion.Net;

namespace Fusion.Http
{
    public class HttpResponse : Response
    {
        protected bool HeadersSent = false;
        
        internal HttpResponse(StateObject state, Request req) : base(state, req)
        {            
            this.Headers = new NameValueCollection();
            this.Headers.Add("Server", "Fusion");
            this.Headers.Add("Date", DateTime.Now.ToUniversalTime().ToString());

            this.Status = "200 OK";
        }

        //
        // Members

        public string Status { get; set; }
        public NameValueCollection Headers { get; protected set; }
        public HttpRequest HttpRequest { get { return (HttpRequest)base.Request; } }

        private bool? _KeepAlive = null;
        internal bool KeepAlive
        {
            get
            {
                if (_KeepAlive == null)
                {
                    if (this.HttpRequest.Headers.AllKeys.Contains("Connection"))
                        _KeepAlive = this.HttpRequest.Headers["Connection"] == "keep-alive";
                    else
                        _KeepAlive = false;
                }
                return _KeepAlive.Value;
            }
        }

        //
        // Methods

        public override void End()
        {
            // Setup headers
            this.Headers["Content-Length"] = this.Buffer.Length.ToString();
            if (!this.KeepAlive)
                this.Headers["Connection"] = "close";

            StringBuilder sb = new StringBuilder();
            sb.Append("HTTP/1.1 " + this.Status + "\r\n");
            foreach (string s in this.Headers)
                sb.Append(s + ": " + this.Headers[s] + "\r\n");
            sb.Append("\r\n");
            byte[] headers = Encoding.ASCII.GetBytes(sb.ToString());
            Buffer = Helpers.ConcatBytes(headers, Buffer);

            // Stream data
            Buffer = Helpers.ConcatBytes(Buffer, Encoding.ASCII.GetBytes("\r\n"));
            this.Stream(Buffer);
            Buffer = new byte[0];

            if (!this.KeepAlive)
                base.End();
        }
        
        public void Ok() { Ok(""); }
        public void Ok(string data)
        {
            this.Headers["Content-Type"] = "text/html; charset=UTF-8";
            this.Write(data);
            this.End();
        }

        /* public override void Stream(byte[] data)
        {
            SendHeaders();
            base.Stream(data);
        } */

        /* public virtual void StreamHeader(string header, int data) { StreamHeader(header, data.ToString()); }
        public virtual void StreamHeader(string header, string data)
        {
            this.Stream(header + ": " + data + "\r\n");
        } */

        //
        // Helpers

        private void SendHeaders()
        {
            if (!HeadersSent)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("HTTP/1.1 " + this.Status + "\r\n");

                foreach (string s in this.Headers)
                    sb.Append(s + ": " + this.Headers[s] + "\r\n");

                sb.Append("\r\n");
                HeadersSent = true;
                this.Stream(sb.ToString());
            }
        }
    }
}
