using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace HostMe
{
    public class StaticContentController : ApiController
    {
        public static string SiteRootPath { private get; set; }

        [EnableCors("*", "*", "*")]
        [Route("{*path}")]
        public HttpResponseMessage GetContent(string path)
        {
            var requestId = Guid.NewGuid();

            Common.WriteLog(requestId, "Got request. path = " + path);
            path = GetNormalizedPath(path);
            Common.WriteLog(requestId, "Path normalized = " + path);

            try
            {
                var content = File.ReadAllBytes(path);
                Common.WriteLog(requestId, "Content read from: " + path);
                var response = new HttpResponseMessage
                {
                    Content = new ByteArrayContent(content)
                };

                var mediaType = MimeMapping.GetMimeMapping(Path.GetFileName(path));

                Common.WriteLog(requestId, "Media Type found = " + mediaType);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
                Common.WriteLog(requestId, "Response sent!");
                return response;
            }
            catch (Exception exception)
            {
                Common.WriteLog(requestId, "Exception happend: " + exception);
                Common.WriteLog(requestId, "Responding with Bad Request!");
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        private static string GetNormalizedPath(string path)
        {
            if (path == null)
                path = "index.html";

            if (SiteRootPath != null)
                path = SiteRootPath + @"\" + path;

            path = Common.NormaliePath(path);
            return path;
        }
    }
}
