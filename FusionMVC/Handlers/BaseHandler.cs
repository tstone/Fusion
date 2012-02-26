using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion.Http;

namespace Fusion.Mvc.Handlers
{
    public class BaseHandler
    {
        public HttpRequest Request { get; set; }
        public HttpResponse Response { get; set; }
        internal RouteInfo RouteInfo { get; set; }
        internal Application App { get; set; }

        public virtual void Begin(HttpRequest req, HttpResponse res, RouteInfo route, Application app)
        {
            this.Request = req;
            this.Response = res;
            this.RouteInfo = route;
            this.App = app;
            
            // Start the show
            this.PreHandle();
        }

        public virtual void PreHandle()
        {
            throw new NotImplementedException();
        }

        public virtual void Error(Exception ex)
        {
            // Pass errors upstream to the Application
            this.App.Error(new WebExceptionEventArgs(ex, this));
        }
    }
}
