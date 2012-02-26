using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion.Http;
using Fusion.Mvc.Handlers;

namespace Fusion.Mvc
{
    public class WebExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; protected set; }
        public HttpRequest Request { get; protected set; }
        public HttpResponse Response { get; protected set; }
        public RouteInfo RouteInfo { get; protected set; }

        public WebExceptionEventArgs(Exception ex, HttpRequest req, HttpResponse res)
        {
            this.Exception = ex;
            this.Request = req;
            this.Response = res;
        }

        public WebExceptionEventArgs(Exception ex, BaseHandler handler)
        {
            this.Exception = ex;
            this.Request = handler.Request;
            this.Response = handler.Response;
            this.RouteInfo = handler.RouteInfo;
        }
    }
    
    // HTTP Errors
    public class BadRequest400 : Exception { }
    public class Unauthorized401 : Exception { }
    public class PaymentRequired402 : Exception { }
    public class Forbidden403 : Exception { }
    public class NotFound404 : Exception { }
    public class MethodNotAllowed405 : Exception { }
    public class NotAcceptable406 : Exception { }
    public class ProxyAuthRequired407 : Exception { }
    public class RequestTimeout408 : Exception { }
    public class Conflict409 : Exception { }
    public class Gone410 : Exception { }
    public class LengthRequired411 : Exception { }
    public class Teapot418 : Exception { }

    // App-specific Errors
    public class RouteParameterTypeNotValid : Exception { }
    public class RouteHandlerIsNotIRouteHandler : Exception { }
    public class MissingMethodArguments : Exception {
        public MissingMethodArguments(string message) : base(message) { }
    }

}

