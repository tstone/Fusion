using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fusion.Mvc
{
    public class RouteInfo
    {
        public string Route { get; internal set; }
        public Regex Pattern { get; internal set; }
        public Match PatternMatch { get; internal set; }
        public Dictionary<string, Type> Types { get; internal set; }

        public RouteInfo()
        {
            Types = new Dictionary<string, Type>();
        }
    }
}
