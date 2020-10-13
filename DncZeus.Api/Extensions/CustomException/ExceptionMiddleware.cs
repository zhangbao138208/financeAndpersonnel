/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace DncZeus.Api.Extensions.CustomException
{
    /// <summary>
    /// 异常中间件
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger)
        {
            _next = next;;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
                _logger.LogError(ex.Message);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var error = new ErrorDetails {
                StatusCode= 500,
                Message=$"资源服务器忙,请稍候再试,原因:{exception.Message}"
            };
            if (exception is UnauthorizeException)
            {
                error.StatusCode= (int)HttpStatusCode.Unauthorized;
                error.Message = "未授权的访问(未登录或者登录已超时)";
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = error.StatusCode;

            return context.Response.WriteAsync(error.ToString());
        }
    }
}
