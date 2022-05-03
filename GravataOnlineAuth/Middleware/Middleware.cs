using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using GravataOnlineAuth.Common;

namespace GravataOnlineAuth.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class HXMiddleware
    {
        private readonly RequestDelegate _next;

        public HXMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            StringValues token;

            context.Request.Headers.TryGetValue("Authorization", out token);
            if (token.Count == 0)
            {
                WriteResponse(context, "User Token is missing", HttpStatusCode.BadRequest);
                return;
            }
            else
            {
                string[] sliptPath = context.Request.Path.Value.Split("/");
                if (sliptPath.Count() < 3)
                {
                    WriteResponse(context, "Controller Route out of default", HttpStatusCode.InternalServerError);
                    return;
                }
                Assembly asm = Assembly.GetExecutingAssembly();
                string nomeController = sliptPath[2] + "Controller";
                Type[] asmTypes = asm.GetTypes();
                Type controller = asmTypes.Where(q => q.Name.StartsWith(nomeController)).FirstOrDefault();
                if (controller == null)
                {
                    WriteResponse(context, "Request route not found", HttpStatusCode.NotFound);
                    return;
                }
                CustomAttributeData attribute = controller.CustomAttributes.Where(q => q.AttributeType.Name == "PermissionAttribute").FirstOrDefault();
                if (attribute == null)
                {
                    WriteResponse(context, "Controller without permission assigned", HttpStatusCode.InternalServerError);
                    return;
                }
                string key = (string)attribute.ConstructorArguments.FirstOrDefault().Value;
                // ConfigureConnection.SetAuthConnection();
                // var login = new HXAuthentication();
                // (bool, User) validToken = login.Validate(token, context.Request.Method, key);
                // login.Dispose();
                // ConfigureConnection.SetMainConnection();
                // if (!validToken.Item1)
                // {
                //     WriteResponse(context, "unauthorized", HttpStatusCode.Unauthorized);
                //     return;
                // }
            }

            await _next.Invoke(context);
        }

        private static void WriteResponse(HttpContext context, string message, HttpStatusCode statusCode)
        {
            context.Response.ContentType = "application/text";
            context.Response.StatusCode = (int)statusCode;
            context.Response.WriteAsync(message).Wait();
        }
    }


    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CustonJwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustonJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HXMiddleware>();
        }

    }




}
