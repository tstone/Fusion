using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion;
using Fusion.Middleware;
using Fusion.Http;
using Fusion.Mvc;
using Fusion.Mvc.Handlers;

namespace Test
{
    public class HelloWorld : RouteHandler
    {
        public void Get(int id)
        {
            /*this.Response.Ok(@"

<html>
    <body>
        <h2>Item #" + id.ToString() + @"</h2>
        <h4>" + DateTime.Now.ToString() + @"</h4>
        <form method=""post"">
            <input name=""title"" />
            <input type=""submit"" value=""Submit"" />
        </form>
    </body>
</html>

            ");*/

            this.Response.Ok("Hello world!");
        }

        public void Post(int id)
        {
            this.Response.Ok("You posted: " + this.Request.Params["title"] + "<br/>And: " + this.Request.Params["asdf"]);
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Application app = new Application(new EnvSettings());
            
            app.AddRoute("/whatever/:id<int>", typeof(HelloWorld));
            app.AddRoute("/public/*", typeof(StaticHandler));
            
            app.Listen(3000);
        }
    }
}
