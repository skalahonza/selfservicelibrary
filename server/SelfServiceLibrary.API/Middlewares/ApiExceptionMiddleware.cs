using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using Pathoschild.Http.Client;

namespace SelfServiceLibrary.API.Middlewares
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiExceptionMiddleware(RequestDelegate next) =>
            _next = next;

        private static bool IsValidJson(string txt, out JsonDocument? document)
        {
            var reader = new Utf8JsonReader(new System.Buffers.ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(txt)));
            try
            {
                return JsonDocument.TryParseValue(ref reader, out document);
            }
            catch (JsonException)
            {
                document = default;
                return false;
            }
        }

        private static Task Handle(HttpContext context, int status, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, string response)
        {
            foreach (var (key, value) in headers.Select(x => (x.Key, x.Value.ToArray())))
                context.Request.Headers.TryAdd(key, new StringValues(value));
            context.Response.StatusCode = status;
            context.Response.ContentType = IsValidJson(response, out _)
                ? "application/json"
                : "text/plain";
            return context.Response.WriteAsync(response);
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiException ex)
            {
                await Handle(context, (int)ex.Status, ex.ResponseMessage.Headers, await ex.Response.AsString());
            }
        }
    }

    public static class ApiExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder app) =>
            app.UseMiddleware(typeof(ApiExceptionMiddleware));
    }
}
