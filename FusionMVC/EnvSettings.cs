using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Fusion.Mvc
{
    public class EnvSettings : ISettings
    {
        #region ISettings Members

        public string Get(string key)
        {
            return Get(key, "");
        }

        public string Get(string key, string def)
        {
            if (key.ToLower() == "root")
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            else
            {
                string value = Environment.GetEnvironmentVariable(key);
                if (string.IsNullOrEmpty(value))
                    return def;
                else
                    return value;
            }
        }

        public T Get<T>(string key, T def)
        {
            string val = Get(key);
            if (string.IsNullOrEmpty(val))
                return def;
            else
                return (T)Convert.ChangeType(val, typeof(T));
        }

        #endregion
    }
}
