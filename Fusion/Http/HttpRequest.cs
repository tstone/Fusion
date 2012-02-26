using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Fusion.Net;

namespace Fusion.Http
{
    public class HttpRequest : Request
    {     
        internal HttpRequest(StateObject state, string headers, string body) : base(state, body)
        {
            _headers = new NameValueCollection();
            this.HeadersRaw = headers;
        }

        private string[] firstLine;
        private string heads;
        private string querystringsRaw;
                
        //
        // Members

        public string HeadersRaw { get; protected set; }

        private string _verb = null;
        public string Verb
        {
            get
            {
                if (_verb == null)
                    parse();
                return _verb;
            }
            internal set { _verb = value; }
        }

        private string _path = null;
        public string Path
        {
            get
            {
                if (_path == null)
                    parse();
                return _path;
            }
            internal set { _protocol = value; }
        }

        private string _protocol = null;
        public string Protocol
        {
            get
            {
                if (_protocol == null)
                    parse();
                return _protocol;
            }
            internal set { _protocol = value; }
        }

        private NameValueCollection _headers;
        public NameValueCollection Headers
        {
            get
            {
                if (_headers == null)
                    parseHeaders();
                return _headers;
            }
            internal set { _headers = value; }
        }
        
        private NameValueCollection _Params;
        public NameValueCollection Params {
            get
            {
                if (_Params == null)
                {
                    _Params = new NameValueCollection();
                    parseParams();
                }
                return _Params;
            }
            internal set { _Params = value; }
        }

        //
        // Helpers

        private void parse()
        {
            string[] s = this.HeadersRaw.Split('\n');
            firstLine = s[0].Split(' ');
            heads = this.HeadersRaw.Substring(s[0].Length + 1);

            _verb = firstLine[0];
            _path = firstLine[1];
            _protocol = firstLine[2].Trim();

            // Console.WriteLine(DateTime.Now.ToString() + ": " + _path);

            if (_path.IndexOf("?") > -1)
            {
                string[] qs = _path.Split('?');
                _path = qs[0];
                querystringsRaw = qs[1];
            }
        }

        private void parseHeaders()
        {
            if (heads == null)
                parse();

            _headers = new NameValueCollection();
            foreach (string h in heads.Split('\n'))
            {
                string[] row = h.Split(':');
                string key = row[0].Trim();
                if (key.Length > 0)
                {
                    string val = h.Substring(key.Length + 1).Trim();
                    _headers[key] = val;
                }
            }
        }

        private void parseParams()
        {
            // GET data
            if (firstLine == null)
                parse();

            int q = firstLine[1].IndexOf("?");
            if (q > -1)
            {
                foreach (string s in firstLine[1].Substring(q + 1).Split('&'))
                {
                    int e = s.IndexOf("=");
                    _Params.Add(s.Substring(0, e).ToLower(), s.Substring(e + 1).Trim());
                }
            }
            
            // POST data
            if (this.Body.Length > 0)
            {
                foreach (string s in this.Body.Split('\n'))
                {
                    int e = s.IndexOf("=");
                    _Params.Add(s.Substring(0, e).ToLower(), s.Substring(e + 1).Trim());
                }
            }
        }
    }
}
