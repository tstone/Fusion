using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Fusion.Http;
using Fusion.Mvc.Handlers;

namespace Fusion.Mvc
{
    public class Routing
    {
        private static Regex patternPlaceholders = new Regex(@":([^/<>]+)(?:<([^>]+)>)?");
        
        private Application App;
        private Dictionary<RouteInfo, BaseHandler> Routes = new Dictionary<RouteInfo, BaseHandler>();
        
        //
        // Construction

        public Routing(Application app)
        {
            this.App = app;
        }

        //
        // Members

        public void Route(HttpRequest req, HttpResponse res)
        {
            foreach (KeyValuePair<RouteInfo, BaseHandler> route in this.Routes)
            {
                Match m = route.Key.Pattern.Match(req.Path);
                if (m.Success)
                {
                    route.Key.PatternMatch = m;
                    route.Value.Begin(req, res, route.Key, this.App);
                    break;
                }
            }
        }
        
        public void Add(string route, BaseHandler handler)
        {
            RouteInfo r = new RouteInfo();

            // Replace placeholders :placeholder<type> with generic regex
            foreach (Match m in patternPlaceholders.Matches(route))
            {
                string name = m.Groups[1].Value;

                // Detect type
                if (m.Groups[2].Success)
                {
                    name = name.Replace(m.Groups[2].Value, "");
                    r.Types.Add(name, GetType(m.Groups[2].Value));
                }
                else
                    r.Types.Add(name, GetType("string"));

                route = route.Replace(m.Value, "(?<" + name + ">[^/]+|$)");
            }
            
            r.Route = route;
            string regexRoute = "^" + route;

            if (!regexRoute.EndsWith("*"))
                regexRoute += "/?$";
            else
                regexRoute = regexRoute.Substring(0, regexRoute.Length - 1);

            r.Pattern = new Regex(regexRoute, RegexOptions.Compiled);
            Routes.Add(r, handler);
        }

        //
        // Helpers

        private Type GetType(string type)
        {
            // Shorthand
            switch (type.ToLower())
            {
                case "int":
                case "int32":
                    return Type.GetType("System.Int32");
                case "int64":
                case "long":
                    return Type.GetType("System.Int64");
                case "single":
                case "float":
                    return Type.GetType("System.Single");
                case "double":
                    return Type.GetType("System.Double");
                case "string":
                    return Type.GetType("System.String");
                default:
                    // Try plain
                    Type t = Type.GetType(type);
                    // Try System. if that didn't work
                    if (t == null)
                        t = Type.GetType("System." + type.Substring(0, 1).ToUpper() + type.Substring(1));
                    return t;
            }

        }
        
    }
}
