using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Fusion.Mvc.Handlers
{
    public class StaticHandler : BaseHandler
    {
        protected string FilePath = string.Empty;
        protected FileStream FileStream;
        byte[] Data = new byte[0];
        byte[] Buffer = new byte[4096];

        public override void PreHandle()
        {
            this.FilePath = App.MapPath(this.Request.Path);

            // Make sure file exists
            if (!File.Exists(FilePath))
                this.Error(new NotFound404());

            this.FileStream = new FileStream(this.FilePath, FileMode.Open);
            this.Get();
        }

        public void Get()
        {
            this.FileStream.BeginRead(Buffer, 0, Buffer.Length, new AsyncCallback(FileRead), null);
        }

        private void FileRead(IAsyncResult ar)
        {
            int read = this.FileStream.EndRead(ar);
            if (read > 0)
            {
                // Concatenate
                byte[] bytes = new byte[Data.Length + Buffer.Length];
                Data.CopyTo(bytes, 0);
                Buffer.CopyTo(bytes, Data.Length);
                Data = bytes;
                // Check for more
                Get();
            }
            else
            {
                this.Response.Headers.Add("Content-Type", Helpers.GetMIMEType(this.FilePath));
                this.Response.Write(Data);
                this.Response.End();
            }
        }
    }
}
