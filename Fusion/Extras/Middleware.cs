using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion.Middleware;
using Fusion.Net;

namespace Fusion.Extras
{
    public static class Middleware
    {
        public static MiddlewareState DebugInbound(Request req, Response res)
        {
            if (req is Http.HttpRequest)
            {
                Http.HttpRequest hreq = (Http.HttpRequest)req;
                Console.WriteLine(" ** Request: " + hreq.Verb + " " + hreq.Path);
            }
            else
                Console.WriteLine(" ** Inbound: " + req.Body + "<END>");
            return new MiddlewareState(req, res);
        }
    }
}
