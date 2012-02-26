using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion.Net;

namespace Fusion.Middleware
{
    public delegate MiddlewareState RequestMiddleware(Request req, Response res);
    public delegate MiddlewareState ResponseMiddleware(Request req, Response res);
    
    public class Middleware
    {
        internal RequestMiddleware[] RequestModules { get; set; }
        internal ResponseMiddleware[] ResponseModules { get; set; }
        private RequestHandler Handler { get; set; }

        public static Middleware Create(RequestHandler handler, RequestMiddleware[] requestModules, ResponseMiddleware[] responseModules)
        {
            Middleware m = new Middleware();
            m.RequestModules = requestModules;
            m.ResponseModules = responseModules;
            m.Handler = handler;
            return m;
        }

        internal void OnRequestHandler(Request req, Response res)
        {
            MiddlewareState state = new MiddlewareState(req, res);

            if (this.RequestModules != null)
            {
                // Invoke each request middleware
                foreach (RequestMiddleware rm in this.RequestModules)
                    state = rm(state.Request, state.Response);
            }

            // Call the original handler with the modified state
            this.Handler(state.Request, state.Response);
        }

        public RequestHandler PassThrough()
        {
            if (_onRequest == null)
                _onRequest = new RequestHandler(this.OnRequestHandler);
            return _onRequest;
        }
        private RequestHandler _onRequest = null;
    }
}
