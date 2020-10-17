/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using DncZeus.Api.Extensions.AuthContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using DncZeus.Api.Entities;
using DncZeus.Api.Services;

namespace DncZeus.Api.Extensions.CustomException
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        // https://tpodolak.com/blog/2017/12/13/asp-net-core-memorycache-getorcreate-calls-factory-method-multiple-times/
        private readonly IMemoryCache _memoryCache;
        private readonly OwnerApiService _ownerApiService;
        private readonly DictionaryService _dictionaryService;


        /// <summary>
        /// 
        /// </summary>
        public CustomAuthorizeAttribute(
            OwnerApiService ownerApiService,
            IMemoryCache memoryCache,
            DictionaryService dictionaryService)
        {
            _ownerApiService = ownerApiService;
            _memoryCache = memoryCache;
            _dictionaryService = dictionaryService;
        }

        /// <summary>
        /// 操作的别名
        /// </summary>
        public string ActionAlias { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public  void OnAuthorization(AuthorizationFilterContext context)
        {
            //return;

            if (AuthContextService.CurrentUser.UserType == UserType.SuperAdministrator)
            {
                return;
            }

            // 以下权限拦截器未现实，所以直接return
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                throw new UnauthorizeException();
            }
            //在方法前使用async 中间件捕捉不到异常
            var  entry =  _ownerApiService.GetApiEntry().Result;
           
           
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var controllerName = controllerActionDescriptor?.ControllerName;
            var actionName = controllerActionDescriptor?.ActionName;
            if (!string.IsNullOrEmpty(ActionAlias))
            {
                actionName = ActionAlias;
            }

            if (entry.Can(controllerName, actionName, out var cName, out var aName)) return;
            cName = cName == "" ? controllerName : cName;
           // aName = aName == "" ? actionName : aName;
           var dic = _dictionaryService.
               GetSYSSeting("operationControlCode");
           var operationCode = dic.Value.Split('|').
               FirstOrDefault(x=>x.Contains(actionName ?? "null",StringComparison.OrdinalIgnoreCase));

           if (operationCode!=null&& operationCode.Split(':').Length==2)
           {
               actionName = operationCode.Split(':')[1];
           }
           
            throw new Forbidden($"对不起您没有对【{cName}】的【{actionName}】权限，请联系管理员添加");
        }
        
        
    }

    
}