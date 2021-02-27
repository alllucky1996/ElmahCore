using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ElmahCore.Mvc.Handlers
{
    internal static class ErrorResourceHandler
    {
        private static readonly string[] ResourceNames =
            typeof(ErrorLogMiddleware).GetTypeInfo().Assembly.GetManifestResourceNames();

        public static async Task ProcessRequest(HttpContext context, string path, string elmahRoot)
        {
            path = path.ToLower();

            var assembly = typeof(ErrorResourceHandler).GetTypeInfo().Assembly;

            var resName = $"{assembly.GetName().Name}.wwwroot.{path.Replace('/', '.').Replace('\\', '.')}";
            if (!path.Contains("."))
            {
                resName = $"{assembly.GetName().Name}.wwwroot.index.html";
                using (var stream2 = assembly.GetManifestResourceStream(resName))
                using (var reader = new StreamReader(stream2 ?? throw new InvalidOperationException()))
                {
                    var html = await reader.ReadToEndAsync();
                    html = html.Replace("ELMAH_ROOT", context.Request.PathBase + elmahRoot);
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(html);
                    return;
                }
            }

            if (!((IList)ResourceNames).Contains(resName))
            {
                context.Response.StatusCode = 404;
                return;
            }

            var ext = Path.GetExtension(path).ToLower();
            if (ext == ".svg")
                context.Response.ContentType = "image/svg+xml";
            if (ext == ".css")
                context.Response.ContentType = "text/css";
            if (ext == ".js")
                context.Response.ContentType = "text/javascript";


            using (var resource = assembly.GetManifestResourceStream(resName))
            {
                if (resource != null) await resource.CopyToAsync(context.Response.Body);
            }
        }
        /// <summary>
        /// Process request
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="path">resource from path in ElmahOptions</param>
        /// <param name="elmahRoot">Path in ElmahOptions</param>
        /// <param name="replaceValues">ReplaceValues in ElmahOptions</param>
        /// <returns></returns>
        public static async Task ProcessRequest(HttpContext context, string path, string elmahRoot, ICollection<KeyValuePair<string, string>> replaceValues)
        {
            path = path.ToLower();

            var assembly = typeof(ErrorResourceHandler).GetTypeInfo().Assembly;

            var resName = $"{assembly.GetName().Name}.wwwroot.{path.Replace('/', '.').Replace('\\', '.')}";
            if (!path.Contains("."))
            {
                resName = $"{assembly.GetName().Name}.wwwroot.index.html";
                using (var stream2 = assembly.GetManifestResourceStream(resName))
                using (var reader = new StreamReader(stream2 ?? throw new InvalidOperationException()))
                {
                    var html = await reader.ReadToEndAsync();
                    html = html.Replace("ELMAH_ROOT", context.Request.PathBase + elmahRoot);
                    if (replaceValues != null && replaceValues.Count > 0)
                    {
                        foreach (var item in replaceValues)
                        {
                            html = html.Replace(item.Key, item.Value);
                        }
                    }
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(html);
                    return;
                }
            }

            if (!((IList)ResourceNames).Contains(resName))
            {
                context.Response.StatusCode = 404;
                return;
            }

            var ext = Path.GetExtension(path).ToLower();
            if (ext == ".svg")
                context.Response.ContentType = "image/svg+xml";
            if (ext == ".css")
                context.Response.ContentType = "text/css";
            if (ext == ".js")
                context.Response.ContentType = "text/javascript";


            using (var resource = assembly.GetManifestResourceStream(resName))
            {
                if (resource != null) await resource.CopyToAsync(context.Response.Body);
            }
        }
    }
}