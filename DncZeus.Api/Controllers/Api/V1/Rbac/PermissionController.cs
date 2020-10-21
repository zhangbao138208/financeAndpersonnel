/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Entities.QueryModels.DncPermission;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.Rbac.Permission;
using DncZeus.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DncZeus.Api.ViewModels.Rbac.DncPermission;
using static DncZeus.Api.Entities.Enums.CommonEnum;
using DncZeus.Api.Extensions.CustomException;
using Microsoft.Data.SqlClient;
using NPOI.POIFS.Properties;
using SqlParameter = Microsoft.Data.SqlClient.SqlParameter;

namespace DncZeus.Api.Controllers.Api.V1.Rbac
{
    /// <summary>
    /// 权限控制器
    /// </summary>
    [Route("api/v1/rbac/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(CustomAuthorizeAttribute))]
    public class PermissionController : Controller
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public PermissionController(DncZeusDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<PermissionJsonModel>>>>
            List(PermissionRequestPayload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            await using (_dbContext)
            {
                var query = _dbContext.DncPermission.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.Name.Contains(payload.Kw.Trim()) ||
                    x.Code.Contains(payload.Kw.Trim()));
                }
                if (payload.IsDeleted > IsDeleted.All)
                {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (payload.Status > Status.All)
                {
                    query = query.Where(x => x.Status == payload.Status);
                }
                if (payload.MenuGuid.HasValue)
                {
                    query = query.Where(x => x.MenuGuid == payload.MenuGuid);
                }
                var list =await query.Paged(payload.CurrentPage, payload.PageSize).
                    Include(x => x.Menu).ToListAsync();
                var totalCount = await query.CountAsync();
                var data = list.Select(_mapper.Map<DncPermission, PermissionJsonModel>);
                /*
                 * .Select(x => new PermissionJsonModel {
                    MenuName = x.Menu.Name,
                    x.
                });
                 */

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建权限
        /// </summary>
        /// <param name="model">权限视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Create(PermissionCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Name.Trim().Length <= 0)
            {
                response.SetFailed("请输入权限名称");
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (await _dbContext.DncPermission.CountAsync(x => x.ActionCode == model.ActionCode 
                                                        && x.MenuGuid == model.MenuGuid) > 0)
                {
                    response.SetFailed("权限操作码已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<PermissionCreateViewModel, DncPermission>(model);
                entity.CreatedOn = DateTime.Now;
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                entity.CreatedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;
                await _dbContext.DncPermission.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑权限
        /// </summary>
        /// <param name="code">权限惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<PermissionEditViewModel>> Edit(string code)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.DncPermission.
                    FirstOrDefaultAsync(x => x.Code == code);
                var response = ResponseModelFactory.CreateInstance;
                var model = _mapper.Map<DncPermission, PermissionEditViewModel>(entity);
                var menu = _dbContext.DncMenu.FirstOrDefault(x => x.Guid == entity.MenuGuid);
                if (menu != null) model.MenuName = menu.Name;
                response.SetData(model);
                return Ok(response);
            }
        }

        /// <summary>
        /// 保存编辑后的权限信息
        /// </summary>
        /// <param name="model">权限视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Edit(PermissionEditViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (await _dbContext.DncPermission.CountAsync(x => x.ActionCode == model.ActionCode && 
                                                        x.Code == model.Code&&x.Name==model.Name) > 0)
                {
                    response.SetFailed("权限操作码已存在");
                    return Ok(response);
                }
                var entity = await _dbContext.DncPermission.
                    FirstOrDefaultAsync(x => x.Code == model.Code);
                if (entity == null)
                {
                    response.SetFailed("权限不存在");
                    return Ok(response);
                }
                entity.Name = model.Name;
                entity.ActionCode = model.ActionCode;
                entity.MenuGuid = model.MenuGuid;
                entity.IsDeleted = model.IsDeleted;
                entity.ModifiedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;
                entity.ModifiedOn = DateTime.Now;
                entity.Status = model.Status;
                entity.Description = model.Description;
                await _dbContext.SaveChangesAsync();
                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="ids">权限ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpDelete("{ids}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Delete(string ids)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }
            response = await UpdateIsDelete(IsDeleted.Yes, ids);
            return Ok(response);
        }

        /// <summary>
        /// 恢复权限
        /// </summary>
        /// <param name="ids">权限ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpPost("{ids}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Recover(string ids)
        {
            var response = await UpdateIsDelete(IsDeleted.No, ids);
            return Ok(response);
        }

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">权限ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Batch(string command, string ids)
        {
            var response = ResponseModelFactory.CreateInstance;
            switch (command)
            {
                case "delete":
                    if (ConfigurationManager.AppSettings.IsTrialVersion)
                    {
                        response.SetIsTrial();
                        return Ok(response);
                    }
                    response = await UpdateIsDelete(IsDeleted.Yes, ids);
                    break;
                case "recover":
                    response = await UpdateIsDelete(IsDeleted.No, ids);
                    break;
                case "forbidden":
                    if (ConfigurationManager.AppSettings.IsTrialVersion)
                    {
                        response.SetIsTrial();
                        return Ok(response);
                    }
                    response = await UpdateStatus(UserStatus.Forbidden, ids);
                    break;
                case "normal":
                    response = await UpdateStatus(UserStatus.Normal, ids);
                    break;
                default:
                    break;
            }
            return Ok(response);
        }

        /// <summary>
        /// 角色-权限菜单树
        /// </summary>
        /// <param name="code">角色编码</param>
        /// <returns></returns>
        [HttpGet("/api/v1/rbac/permission/permission_tree/{code}")]
        public async Task<ActionResult<PermissionTreeModel>> PermissionTree(string code)
        {
            var response = ResponseModelFactory.CreateInstance;
            await using (_dbContext)
            {
                var role = await _dbContext.DncRole.FirstOrDefaultAsync(x => x.Code == code);
                if (role == null)
                {
                    response.SetFailed("角色不存在");
                    return Ok(response);
                }
                var menu = await _dbContext.DncMenu.
                    Where(x => x.IsDeleted == IsDeleted.No && x.Status == Status.Normal).
                    OrderBy(x => x.CreatedOn).ThenBy(x => x.Sort)
                    .Select(x => new PermissionMenuTree
                    {
                        Guid = x.Guid,
                        ParentGuid = x.ParentGuid,
                        Title = x.Name
                    }).ToListAsync();
                //DncPermissionWithAssignProperty
                // var sql =  @"-- noinspection SqlNoDataSourceInspectionForFile
                //
                // SELECT P.Code,P.MenuGuid,P.Name,P.ActionCode,ISNULL(S.RoleCode,'') AS RoleCode,(CASE WHEN S.PermissionCode IS NOT NULL THEN 1 ELSE 0 END) AS IsAssigned FROM DncPermission AS P 
                // LEFT JOIN (SELECT * FROM DncRolePermissionMapping AS RPM WHERE RPM.RoleCode={0}) AS S 
                // ON S.PermissionCode= P.Code
                // WHERE P.IsDeleted=0 AND P.Status=1";
                var permissionList = await GetPermissions(role);
                var dncPermissionWithAssignProperties = permissionList as DncPermissionWithAssignProperty[] ?? permissionList.ToArray();
                var tree = 
                    menu.FillRecursive(dncPermissionWithAssignProperties, Guid.Empty, role.IsSuperAdministrator);


                if (!role.IsSuperAdministrator)
                {
                   //var removeTree=new List<PermissionMenuTree>();
                    var parent = await _dbContext.DncRole.FindAsync(role.ParentCode);
                    var pList = await GetPermissions(parent);
                    var pCode = pList.
                        Where(x => x.IsAssigned == 1).
                        Select(x => x.Code).ToArray();
                    SetUnauthorized(tree);
                    void SetUnauthorized(IEnumerable<PermissionMenuTree> pTree)
                    {
                        
                        foreach (var t in pTree)
                        {
                            var removePer=new List<PermissionElement>(t.Permissions.ToArray());
                            foreach (var element in t.Permissions.
                                Where(element => !pCode.Contains(element.Code,StringComparer.OrdinalIgnoreCase)))
                            {
                                removePer.Remove(element);
                            }

                            t.Permissions = removePer;
                            SetUnauthorized(t.Children);
                        }
                    }

                    tree = RemoveUnauthoirzed( tree);
                    List<PermissionMenuTree> RemoveUnauthoirzed( ICollection<PermissionMenuTree> plist)
                    {
                        var copy = new List<PermissionMenuTree>(plist.ToArray());
                        foreach (var p in plist)
                        {
                           p.Children= RemoveUnauthoirzed( p.Children);
                            if (p.Children.Count==0&& p.Permissions.Count==0)
                            {
                                copy.Remove(p);
                            }
                        }

                       return copy;


                    }
                   
                }
                
                response.SetData(new { tree, selectedPermissions = dncPermissionWithAssignProperties.Where(x => x.IsAssigned == 1).Select(x => x.Code) });
            }

            return Ok(response);
        }
        
        

        #region 私有方法


        private async Task<IEnumerable<DncPermissionWithAssignProperty>> GetPermissions(DncRole role)
        {
            var sql =  @"-- noinspection SqlNoDataSourceInspectionForFile
                
                SELECT P.Code,P.MenuGuid,P.Name,P.ActionCode,S.RoleCode AS RoleCode,(CASE WHEN S.PermissionCode IS NOT NULL THEN 1 ELSE 0 END) AS IsAssigned FROM DncPermission AS P 
                LEFT JOIN (SELECT * FROM DncRolePermissionMapping AS RPM WHERE RPM.RoleCode={0}) AS S 
                ON S.PermissionCode= P.Code
                WHERE P.IsDeleted=0 AND P.Status=1";
            if (role.IsSuperAdministrator)
            {
                sql =
                    @"-- noinspection SqlNoDataSourceInspectionForFile
                        
                        SELECT P.Code,P.MenuGuid,P.Name,P.ActionCode,'SUPERADM' AS RoleCode,(CASE WHEN P.Code IS NOT NULL THEN 1 ELSE 0 END) AS IsAssigned FROM DncPermission AS P 
                        WHERE P.IsDeleted=0 AND P.Status=1";
            }
            var permissionList =await _dbContext.DncPermissionWithAssignProperty.
                FromSqlRaw(sql, role.Code).ToListAsync();
            //为了mysql 和SqlServer通用
            permissionList.ForEach(p => p.RoleCode ??= "");
            return permissionList;
        }
        
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">权限ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private  async Task<ResponseModel> UpdateIsDelete(IsDeleted isDeleted, string ids)
        {
            await using (_dbContext)
            {
                // var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                // var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                // var sql = $"UPDATE DncPermission SET IsDeleted=@IsDeleted WHERE Code IN ({parameterNames})";
                // parameters.Add(new SqlParameter("@IsDeleted", (int)isDeleted));
                // await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE DncPermission SET IsDeleted={(int)isDeleted} WHERE Code IN ({formatIds})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="status">权限状态</param>
        /// <param name="ids">权限ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateStatus(UserStatus status, string ids)
        {
            await using (_dbContext)
            {
                // var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                // var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                // var sql = $"UPDATE DncPermission SET Status=@Status WHERE Code IN ({parameterNames})";
                // parameters.Add(new SqlParameter("@Status", status));
                // await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE DncPermission SET Status={(int)status} WHERE Code IN ({formatIds})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion
    }
    
    
    /// <summary>
    /// 
    /// </summary>
    public static class PermissionTreeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="menus">菜单集合</param>
        /// <param name="permissions">权限集合</param>
        /// <param name="parentGuid">父级菜单GUID</param>
        /// <param name="isSuperAdministrator">是否为超级管理员角色</param>
        /// <returns></returns>
        public static List<PermissionMenuTree> FillRecursive(this IEnumerable<PermissionMenuTree> menus, IEnumerable<DncPermissionWithAssignProperty> permissions, Guid? parentGuid, bool isSuperAdministrator = false)
        {
            var permissionMenuTrees = menus as PermissionMenuTree[] ?? menus.ToArray();
            return permissionMenuTrees.Where(x => x.ParentGuid == parentGuid)
                .Select(item =>
                {
                    var dncPermissionWithAssignProperties = permissions as DncPermissionWithAssignProperty[] ?? permissions.ToArray();
                    return new PermissionMenuTree
                    {
                        AllAssigned = isSuperAdministrator || (dncPermissionWithAssignProperties
                            .Where(x => x.MenuGuid == item.Guid).Count(x => x.IsAssigned == 0) == 0),
                        Expand = true,
                        Guid = item.Guid,
                        ParentGuid = item.ParentGuid,
                        Permissions = dncPermissionWithAssignProperties.Where(x => x.MenuGuid == item.Guid).Select(x =>
                            new PermissionElement
                            {
                                Name = x.Name, Code = x.Code,
                                IsAssignedToRole = IsAssigned(x.IsAssigned, isSuperAdministrator)
                            }).OrderBy(x=>x.Name).ToList(),
                        Title = item.Title,
                        Children = FillRecursive(permissionMenuTrees, dncPermissionWithAssignProperties, item.Guid)
                    };
                })
                .ToList();
        }

        private static bool IsAssigned(int isAssigned, bool isSuperAdministrator)
        {
            if (isSuperAdministrator)
            {
                return true;
            }
            return isAssigned == 1;
        }

        //public static List<PermissionMenuTree> FillRecursive(this List<PermissionMenuTree> menus, List<DncPermissionWithAssignProperty> permissions, Guid? parentGuid)
        //{
        //    List<PermissionMenuTree> recursiveObjects = new List<PermissionMenuTree>();

        //    return recursiveObjects;
        //}
    }
}