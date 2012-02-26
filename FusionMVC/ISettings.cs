using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fusion.Mvc
{
    public interface ISettings
    {
        string Get(string key);
        string Get(string key, string def);
        T Get<T>(string key, T def);
    }
}
