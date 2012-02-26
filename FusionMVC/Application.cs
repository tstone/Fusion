using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using Fusion.Http;
using Fusion.Mvc.Handlers;

namespace Fusion.Mvc
{
    public class Application
    {
        public HttpServer Server { get; protected set; }
        public Routing Routes { get; protected set; }
        public ISettings Settings { get; protected set; }

        //
        // Construction

        private void Init()
        {
            this.Routes = new Routing(this);
            this.Server = new HttpServer(new HttpRequestHandler(this.Routes.Route));
        }

        public Application(ISettings settings)
        {
            this.Settings = settings;
            Init();
        }

        public Application()
        {
            Init();
        }
        
        //
        // Members

        public string MapPath(string path)
        {
            path = path.Replace("/", "\\");

            string root = this.Settings.Get("root");
            if (root.EndsWith("\\"))
                root = root.Substring(0, root.Length - 1);
            if (root.StartsWith("file:"))
                root = root.Substring(6);

            return root + path;
        }

        internal void Error(WebExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
        }

        //
        // Syntatic sugar

        public void AddRoute(string route, BaseHandler handler)
        {
            this.Routes.Add(route, handler);
        }

        public void Listen(int port)
        {
            this.Server.Listen(port);
        }

        public void Listen(IPAddress ip, int port)
        {
            this.Server.Listen(ip, port);
        }

    }
}
