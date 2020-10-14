/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Entities.Enums;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.Rbac.Role;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.Rbac.DncRole;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DncZeus.Api.Controllers.Api.V1.Rbac
{
    /// <summary>
    /// 
    /// </summary>
    //[CustomAuthorize]
    [Route("api/v1/rbac/[controller]/[action]")]
    [ApiController]
    [CustomAuthorize]
    public class RoleController : ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public RoleController(DncZeusDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<RoleJsonModel>>>> 
            List(RoleRequestPayload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            await using (_dbContext)
            {
                var query = _dbContext.DncRole.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.Name.Contains(payload.Kw.Trim()) || x.Code.Contains(payload.Kw.Trim()));
                }
                if (payload.IsDeleted > CommonEnum.IsDeleted.All)
                {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (payload.Status > CommonEnum.Status.All)
                {
                    query = query.Where(x => x.Status == payload.Status);
                }
                var list =await query.Paged(payload.CurrentPage, payload.PageSize).ToListAsync();
                var totalCount = await query.CountAsync(); 
                var data = list.Select(_mapper.Map<DncRole, RoleJsonModel>);

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="model">角色视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Create(RoleCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Name.Trim().Length <= 0)
            {
                response.SetFailed("请输入角色名称");
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (_dbContext.DncRole.Count(x => x.Name == model.Name) > 0)
                {
                    response.SetFailed("角色已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<RoleCreateViewModel, DncRole>(model);
                entity.CreatedOn = DateTime.Now;
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                entity.IsSuperAdministrator = false;
                entity.IsBuiltin = false;
                entity.CreatedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;
                await _dbContext.DncRole.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="code">角色惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel<RoleCreateViewModel>>> Edit(string code)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.DncRole.
                    FirstOrDefaultAsync(x => x.Code == code);
                var response = ResponseModelFactory.CreateInstance;
                response.SetData(_mapper.Map<DncRole, RoleCreateViewModel>(entity));
                return Ok(response);
            }
        }

        /// <summary>
        /// 保存编辑后的角色信息
        /// </summary>
        /// <param name="model">角色视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Edit(RoleCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (await _dbContext.DncRole.CountAsync(x => x.Name == model.Name 
                                                             && x.Code != model.Code) > 0)
                {
                    response.SetFailed("角色已存在");
                    return Ok(response);
                }

                var entity = _dbContext.DncRole.FirstOrDefault(x => x.Code == model.Code);

                if (entity != null && entity.IsSuperAdministrator && !AuthContextService.IsSupperAdministator)
                {
                    response.SetFailed("没有足够的权限");
                    return Ok(response);
                }

                if (entity != null)
                {
                    entity.Name = model.Name;
                    entity.IsDeleted = model.IsDeleted;
                    entity.ModifiedByUserGuid = AuthContextService.CurrentUser.Guid;
                    entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;
                    entity.ModifiedOn = DateTime.Now;
                    entity.Status = model.Status;
                    entity.Description = model.Description;
                }

                await _dbContext.SaveChangesAsync();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="ids">角色ID,多个以逗号分隔</param>
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
            response = await UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
            return Ok(response);
        }

        /// <summary>
        /// 恢复角色
        /// </summary>
        /// <param name="ids">角色ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpPost("{ids}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Recover(string ids)
        {
            var response = await UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
            return Ok(response);
        }

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">角色ID,多个以逗号分隔</param>
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
                    response = await UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
                    break;
                case "recover":
                    response = await UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
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
        /// 为指定角色分配权限
        /// </summary>
        /// <param name="payload">角色分配权限的请求载体类</param>
        /// <returns></returns>
        [HttpPost("/api/v1/rbac/role/assign_permission")]
        public async Task<ActionResult<ResponseModel>> AssignPermission(RoleAssignPermissionPayload payload)
        {
            var response = ResponseModelFactory.CreateInstance;
            await using (_dbContext)
            {
                var role = _dbContext.DncRole.FirstOrDefault(x => x.Code == payload.RoleCode);
                if (role == null)
                {
                    response.SetFailed("角色不存在");
                    return Ok(response);
                }
                // 如果是超级管理员，则不写入到角色-权限映射表(在读取时跳过角色权限映射，直接读取系统所有的权限)
                if (role.IsSuperAdministrator)
                {
                    response.SetSuccess();
                    return Ok(response);
                }
                //先删除当前角色原来已分配的权限
                await _dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM DncRolePermissionMapping WHERE RoleCode={0}", payload.RoleCode);
                if (payload.Permissions == null || (payload.Permissions == null && payload.Permissions.Count <= 0))
                    return Ok(response);
                {
                    var permissions = payload.Permissions.Select(x => new DncRolePermissionMapping
                    {
                        CreatedOn = DateTime.Now,
                        PermissionCode = x.Trim(),
                        RoleCode = payload.RoleCode.Trim()
                    });
                    await _dbContext.DncRolePermissionMapping.AddRangeAsync(permissions);
                    await _dbContext.SaveChangesAsync();
                }

            }
            return Ok(response);
        }

        /// <summary>
        /// 获取指定用户的角色列表
        /// </summary>
        /// <param name="guid">用户GUID</param>
        /// <returns></returns>
        [HttpGet("/api/v1/rbac/role/find_list_by_user_guid/{guid}")]
        public async Task<IActionResult> FindListByUserGuid(Guid guid)
        {
            var response = ResponseModelFactory.CreateInstance;
            await using (_dbContext)
            {
                //有N+1次查询的性能问题
                //var query = _dbContext.DncUser
                //    .Include(r => r.UserRoles)
                //    .ThenInclude(x => x.DncRole)
                //    .Where(x => x.Guid == guid);
                //var roles = query.FirstOrDefault().UserRoles.Select(x => new
                //{
                //    x.DncRole.Code,
                //    x.DncRole.Name
                //});
                var sql = @"-- noinspection SqlNoDataSourceInspection
                
                SELECT R.* FROM DncUserRoleMapping AS URM
                INNER JOIN DncRole AS R ON R.Code=URM.RoleCode
                WHERE URM.UserGuid={0}";
                var query = await _dbContext.DncRole.FromSqlRaw(sql, guid).ToListAsync();
                var assignedRoles = query.ToList().Select(x => x.Code).ToList();
                var roles = _dbContext.DncRole.
                    Where(x => x.IsDeleted == CommonEnum.IsDeleted.No &&
                               x.Status == CommonEnum.Status.Normal).ToListAsync().Result.
                    Select(x => new { label = x.Name, key = x.Code });
                response.SetData(new { roles, assignedRoles });
                return Ok(response);
            }
        }

        /// <summary>
        /// 查询所有角色列表(只包含主要的字段信息:name,code)
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/v1/rbac/role/find_simple_list")]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<SimpleModel>>>> FindSimpleList()
        {
            var response = ResponseModelFactory.CreateInstance;
            await using (_dbContext)
            {
                var roles =
                    await _dbContext.DncRole.Where(x => x.IsDeleted == CommonEnum.IsDeleted.No && 
                                                  x.Status == CommonEnum.Status.Normal).
                        Select(x => new { x.Name, x.Code }).ToListAsync();
                response.SetData(roles);
            }
            return Ok(response);
        }

        #region 私有方法

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">角色ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids)
        {
            await using (_dbContext)
            {
                // var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                // var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                // var sql = $"UPDATE DncRole SET IsDeleted=@IsDeleted WHERE Code IN ({parameterNames})";
                // parameters.Add(new SqlParameter("@IsDeleted", (int)isDeleted));
                // await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE DncRole SET IsDeleted={(int)isDeleted} WHERE Code IN ({formatIds})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="status">角色状态</param>
        /// <param name="ids">角色ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateStatus(UserStatus status, string ids)
        {
            await using (_dbContext)
            {
                // var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                // var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                // var sql = $"UPDATE DncRole SET Status=@Status WHERE Code IN ({parameterNames})";
                // parameters.Add(new SqlParameter("@Status", (int)status));
                // await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE DncRole SET Status={(int)status} WHERE Code IN ({formatIds})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion
    }
}