using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using log4net;

namespace HostMe
{
    public class StaticContentController : ApiController
    {
        private readonly ILog _logger = Logger.GetLogger();
        public static string SiteRootPath { private get; set; }

        [EnableCors("*", "*", "*")]
        [Route("{*path}")]
        public HttpResponseMessage GetContent(string path)
        {
            var requestId = Guid.NewGuid();
            _logger.Info("Got request. path = " + path);
            path = GetNormalizedPath(path);
            _logger.Info("Path normalized = " + path);

            try
            {
                var content = File.ReadAllBytes(path);
                _logger.Info("Content read from: " + path);
                var response = new HttpResponseMessage
                {
                    Content = new ByteArrayContent(content)
                };

                var mediaType = MimeMapping.GetMimeMapping(Path.GetFileName(path));

                _logger.Info("Media Type found = " + mediaType);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
                _logger.Info("Response sent!");
                return response;
            }
            catch (Exception exception)
            {
                _logger.Warn(path + " could not be parsed.", exception);
                _logger.Warn("Responding with Bad Request!");
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        private static string GetNormalizedPath(string path)
        {
            if (path == null)
                path = "index.html";

            if (SiteRootPath != null)
                path = SiteRootPath + @"\" + path;

            path = PathNormalizer.NormaliePath(path);
            return path;
        }
    }
}
