using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Fusion.Http;

namespace Fusion.Mvc.Handlers
{
    public class RouteHandler : BaseHandler
    {
        protected virtual void InvokeHandle()
        {            
            // Method named is based on the verb of the request
            string methodName = this.Request.Verb.ToLower();
            methodName = methodName.Substring(0, 1).ToUpper() + methodName.Substring(1);

            // Build up list of params
            List<object> handleParams = new List<object>();
            foreach (KeyValuePair<string, Type> param in this.RouteInfo.Types)
            {
                try
                {
                    object val = RouteInfo.PatternMatch.Groups[param.Key].Value;
                    handleParams.Add(Convert.ChangeType(val, param.Value));
                }
                catch { this.Error(new RouteParameterTypeNotValid()); }
            }

            // Invoke Handle method using reflection based on parameters
            MethodInfo method = this.GetType().GetMethod(methodName);
            if (method != null)
            {
                try { method.Invoke(this, handleParams.ToArray()); }
                catch (Exception ex)
                {
                    if (ex is TargetParameterCountException)
                        this.Error(new MissingMethodArguments("Missing input parameter(s) on " + this.GetType().ToString() + "." + methodName + "()."));
                    else
                        this.Error(ex);
                }
            }
            else
                this.Error(new RouteParameterTypeNotValid());
        }

        public override void PreHandle()
        {
            InvokeHandle();
        }

    }
}
