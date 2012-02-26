using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion.Net;

namespace Fusion.Middleware
{
    public class MiddlewareState
    {
        public Request Request { get; internal set; }
        public Response Response { get; internal set; }

        public MiddlewareState(Request req, Response res)
        {
            this.Request = req;
            this.Response = res;
        }
    }
}
