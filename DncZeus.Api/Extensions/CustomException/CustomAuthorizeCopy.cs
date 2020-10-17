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
using DncZeus.Api.Entities;
using DncZeus.Api.Services;

namespace DncZeus.Api.Extensions.CustomException
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CustomAuthorizeCopyAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        // https://tpodolak.com/blog/2017/12/13/asp-net-core-memorycache-getorcreate-calls-factory-method-multiple-times/
        private IMemoryCache _memoryCache;


        /// <summary>
        /// 
        /// </summary>
        public CustomAuthorizeCopyAttribute()
        {
        }

        /// <summary>
        /// 操作的别名
        /// </summary>
        public string ActionAlias { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            return;

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

            // var ownerApiService = GlobalAppConst.ServiceProvider.CreateScope().ServiceProvider.
            //     GetService(typeof(OwnerApiService)) as OwnerApiService;
            
            var ownerApiService = GlobalAppConst.ServiceProvider.
                GetService(typeof(OwnerApiService)) as OwnerApiService;
            var  entry = await ownerApiService.GetApiEntry();
           
            
            // _memoryCache = (IMemoryCache)context.HttpContext.RequestServices.GetService(typeof(IMemoryCache));
            // _memoryCache.GetOrCreate("CK_PERMISSION_" + 
            //                          AuthContextService.CurrentUser.LoginName,  (cache) =>
            // { 
            //    
            //
            //     
            //     //entry = new OwnedApiPermission();
            //     cache.SlidingExpiration = TimeSpan.FromMinutes(30);
            //     return entry;
            // });
            // var optionsBuilder = new DbContextOptionsBuilder<DncZeusDbContext>();
            // var builder = new ConfigurationBuilder()
            //     .SetBasePath(Directory.GetCurrentDirectory())
            //     .AddJsonFile("appsettings.json");
            // var configuration = builder.Build();
            // var connectionString = configuration.GetConnectionString("MYSQLConnection");
            // optionsBuilder.UseMySql(connectionString);
            // var dbContext = new DncZeusDbContext(optionsBuilder.Options);
            // //TODO: load real permission list from db
            // using (dbContext)
            // {
            //     var roles = dbContext.DncUserRoleMapping.Where(x => x.UserGuid == AuthContextService.CurrentUser.Guid)
            //         .Select(x => x.RoleCode).ToListAsync().Result;
            //
            //     var pm = dbContext.DncRolePermissionMapping.Where(x => roles.Contains(x.RoleCode))
            //         .Select(x => x.PermissionCode).ToListAsync().Result;
            //
            //     var list = dbContext.DncPermission.Where(x => pm.Contains(x.Code)).ToListAsync().Result;
            //     var gp = list.GroupBy(x => new {x.MenuGuid})
            //         .Select(group => new
            //         {
            //             group.Key
            //         }).ToList();
            //     foreach (var permission in gp)
            //     {
            //         var canAccess = new CanAccess();
            //         canAccess.Controller = dbContext.DncMenu
            //             .FindAsync(permission.Key.MenuGuid).Result.Alias;
            //         canAccess.Actions = dbContext.DncPermission
            //             .Where(x => x.MenuGuid == permission.Key.MenuGuid).Select(x => x.ActionCode)
            //             .ToListAsync().Result;
            //
            //         entry.CanAccesses.Add(canAccess);
            //     }
            // }

            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            string controllerName = controllerActionDescriptor?.ControllerName;
            string actionName = controllerActionDescriptor?.ActionName;
            if (!string.IsNullOrEmpty(ActionAlias))
            {
                actionName = ActionAlias;
            }

            if (!entry.Can(controllerName, actionName,out var cName,out var aName))
            {
                throw new Forbidden($"对不起您没有对{cName}的{aName}操作权限，请联系管理员添加");
            }
        }
        
        
    }

    
}