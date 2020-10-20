using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.ViewModels.System.Dictionary;
using DncZeus.Api.ViewModels.System.DicType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using Microsoft.EntityFrameworkCore;

namespace DncZeus.Api.Cache
{
    public class CacheData
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;

        public CacheData(DncZeusDbContext dbContext,IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public  List<DictionaryJsonModel> SystemDictionary
        {
            get
            {

                var list = CacheManager.GetCache<List<DictionaryJsonModel>>("DncZeusDic");
                if (list == null)
                {
                    //using (_dbContext)
                    //{
                    //    list = _dbContext.SystemDictionary.ToList();
                    //}
                    list = _dbContext.SystemDictionary.Select(_mapper.Map<SystemDictionary, DictionaryJsonModel>).ToList(); ;

                    CacheManager.SetCache("DncZeusDic", list);
                }
                return list;
            }
        }

        public List<DicTypeJsonModel> SysDictType
        {
            get
            {

                var list = CacheManager.GetCache<List<DicTypeJsonModel>>("DncZeusSysDictType");
                if (list == null)
                {
                    //using (_dbContext)
                    //{
                    //    list = _dbContext.SystemDicType.ToList();
                    //}
                    list = _dbContext.SystemDicType.Select(_mapper.Map<SystemDicType, DicTypeJsonModel>).ToList(); 

                    CacheManager.SetCache("DncZeusSysDictType", list);
                }
                return list;
            }
        }
        
        public OwnedApiPermission ApiEntry
        {
            get
            {

                var retEntry = CacheManager.GetCache<OwnedApiPermission>($"{AuthContextService.CurrentUser.Guid.ToString().ToLower()}OwnedApiPermission");
                if (retEntry != null) return retEntry;
                var entry = new OwnedApiPermission();
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
                //TODO: load real permission list from db
                
                    var roles = _dbContext.DncUserRoleMapping.Where(x => x.UserGuid == AuthContextService.CurrentUser.Guid)
                        .Select(x => x.RoleCode).ToListAsync().Result;

                    var pm = _dbContext.DncRolePermissionMapping.Where(x => roles.Contains(x.RoleCode))
                        .Select(x => x.PermissionCode).ToListAsync().Result;

                    var list = _dbContext.DncPermission.Where(x => pm.Contains(x.Code)).ToListAsync().Result;
                    var gp = list.GroupBy(x => new { x.MenuGuid })
                        .Select(group => new
                        {
                            @group.Key
                        }).ToList();
                    foreach (var canAccess in gp.Select(permission => new CanAccess
                    {
                        Controller = _dbContext.DncMenu
                            .FindAsync(permission.Key.MenuGuid).Result.Alias,
                        ControllerDisplayName = _dbContext.DncMenu
                            .FindAsync(permission.Key.MenuGuid).Result.Name,
                        Actions = _dbContext.DncPermission
                            .Where(x => x.MenuGuid == permission.Key.MenuGuid).Select(x=>new CanAccessAction
                            {
                                Code = x.ActionCode,
                                Name = x.Name
                            })
                            .ToListAsync().Result
                    }))
                    {
                        entry.CanAccesses.Add(canAccess);
                    }

                    CacheManager.SetCache($"{AuthContextService.CurrentUser.Guid.ToString().ToLower()}OwnedApiPermission", entry);
                    retEntry = entry;
                
                return retEntry;
            }
        }

        public void ClearApiEntryCache(List<string> users)
        {
            users.ForEach(u =>
            {
                CacheManager.RemoveCache($"{u.ToLower()}OwnedApiPermission");
            });
            
        }

        public void ClearDictionaryCache()
        {
            CacheManager.RemoveCache("DncZeusSysDictType");
            CacheManager.RemoveCache("DncZeusDic");
        }
    }
}
