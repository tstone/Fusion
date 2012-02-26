using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion.Http;

namespace Fusion.Mvc.Handlers
{
    public interface IRouteHandler
    {
        void Begin(HttpRequest req, HttpResponse res, RouteInfo route, Application app);
        void PreHandle();
        void Error(Exception ex);
    }
}
