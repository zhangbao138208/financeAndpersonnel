using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Entities.Enums;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.DataAccess;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.Rbac.User;
using DncZeus.Api.ViewModels.Rbac.DncUser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using DncZeus.Api.Utils.Encryption;
using System.Text.Unicode;
using System.Text;
using System.Threading.Tasks;
using DncZeus.Api.Utils;

/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

namespace DncZeus.Api.Controllers.Api.V1.Rbac
{
    /// <summary>
    /// 
    /// </summary>
    //[CustomAuthorize]
    [Route("api/v1/rbac/[controller]/[action]")]
    [ApiController]
    //[CustomAuthorize]
    public class UserController : ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly RSAHelper _rSaHelper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        /// <param name="rSaHelper"></param>
        public UserController(DncZeusDbContext dbContext, IMapper mapper, RSAHelper rSaHelper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _rSaHelper = rSaHelper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<UserJsonModel>>>> 
            List(UserRequestPayload payload)
        {
            await using (_dbContext)
            {
                var query = _dbContext.DncUser.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.LoginName.Contains(payload.Kw.Trim()) || x.DisplayName.Contains(payload.Kw.Trim()));
                }
                if (payload.IsDeleted > CommonEnum.IsDeleted.All)
                {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (payload.Status > UserStatus.All)
                {
                    query = query.Where(x => x.Status == payload.Status);
                }

                if (payload.FirstSort != null)
                {
                    query = query.OrderBy(payload.FirstSort.Field, payload.FirstSort.Direct == "DESC");
                }
                var list = await query.Paged(payload.CurrentPage, payload.PageSize).ToListAsync();
                var totalCount = await query.CountAsync();
                var data = list.Select(_mapper.Map<DncUser, UserJsonModel>);
                var response = ResponseModelFactory.CreateResultInstance;
                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="model">用户视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Create(UserCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.LoginName.Trim().Length <= 0)
            {
                response.SetFailed("请输入登录名称");
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (await _dbContext.DncUser.CountAsync(x => x.LoginName ==
                                                             model.LoginName) > 0)
                {
                    response.SetFailed("登录名已存在");
                    return Ok(response);
                }
                //RSAHelper rSAHelper = new RSAHelper
                //    (RSAType.RSA,Encoding.UTF8, CeyhConfiguration.TheRSASetting.Private,CeyhConfiguration.TheRSASetting.Public);
                var entity = _mapper.Map<UserCreateViewModel, DncUser>(model);
                entity.CreatedOn = DateTime.Now;
                entity.Password = _rSaHelper.Encrypt(model.Password);
                entity.Guid = Guid.NewGuid();
                entity.Status = model.Status;
                await _dbContext.DncUser.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="guid">用户GUID</param>
        /// <returns></returns>
        [HttpGet("{guid}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel<UserEditViewModel>>> Edit(Guid guid)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.DncUser.FirstOrDefaultAsync(x => x.Guid == guid);
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<DncUser, UserEditViewModel>(entity);
                if (entity != null) resEntity.Password = _rSaHelper.Decrypt(entity.Password);
                response.SetData(resEntity);
                return Ok(response);
            }
        }

        /// <summary>
        /// 保存编辑后的用户信息
        /// </summary>
        /// <param name="model">用户视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Edit(UserEditViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }

            await using (_dbContext)
            {
                var entity = await _dbContext.DncUser.
                    FirstOrDefaultAsync(x => x.Guid == model.Guid);
                if (entity == null)
                {
                    response.SetFailed("用户不存在");
                    return Ok(response);
                }
                //RSAHelper rSAHelper = new RSAHelper
                //   (RSAType.RSA, Encoding.UTF8, CeyhConfiguration.TheRSASetting.Private, CeyhConfiguration.TheRSASetting.Public);
                entity.DisplayName = model.DisplayName;
                entity.IsDeleted = model.IsDeleted;
                entity.IsLocked = model.IsLocked;
                entity.ModifiedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;
                entity.ModifiedOn = DateTime.Now;
                entity.TelegramBotToken = model.TelegramBotToken;
                entity.TelegramChatId = model.TelegramChatId;
                entity.Password = _rSaHelper.Encrypt(model.Password);
                entity.Avatar = model.Avatar;
                entity.Status = model.Status;
                entity.UserType = model.UserType;
                entity.Description = model.Description;
                await _dbContext.SaveChangesAsync();
                response = ResponseModelFactory.CreateInstance;
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="ids">用户GUID,多个以逗号分隔</param>
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
        /// 恢复用户
        /// </summary>
        /// <param name="ids">用户GUID,多个以逗号分隔</param>
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
        /// <param name="ids">用户ID,多个以逗号分隔</param>
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

        #region 用户-角色
        /// <summary>
        /// 保存用户-角色的关系映射数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("/api/v1/rbac/user/save_roles")]
        public async Task<ActionResult<ResponseModel>> SaveRoles(SaveUserRolesViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            var roles = model.AssignedRoles.
                Select(x => new DncUserRoleMapping
            {
                UserGuid = model.UserGuid,
                CreatedOn = DateTime.Now,
                RoleCode = x.Trim()
            }).ToList();
            await _dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM DncUserRoleMapping WHERE UserGuid={0}", model.UserGuid);
            var success = true;
            if (roles.Count > 0)
            {
                await _dbContext.DncUserRoleMapping.AddRangeAsync(roles);
                success = await _dbContext.SaveChangesAsync() > 0;
            }

            if (success)
            {
                response.SetSuccess();
            }
            else
            {
                response.SetFailed("保存用户角色数据失败");
            }
            return Ok(response);
        }

        /// <summary>
        /// 查询所有步骤列表(只包含主要的字段信息:name,code)
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/v1/rbac/user/find_simple_list")]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<SimpleModel>>>> FindSimpleList()
        {
            var response = ResponseModelFactory.CreateInstance;
            await using (_dbContext)
            {
                var users = await _dbContext.DncUser.
                    Where(x => x.IsDeleted == CommonEnum.IsDeleted.No && 
                               x.Status == UserStatus.Normal)
                    .Select(x => new { name= x.DisplayName,code= x.Guid }).ToListAsync();
                response.SetData(users);
            }
            return Ok(response);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">用户ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids)
        {
            await using (_dbContext)
            {
                //var idList = ids.Split(",").ToList();
                ////idList.ForEach(x => {
                ////  _dbContext.Database.ExecuteSqlCommand($"UPDATE DncUser SET IsDeleted=1 WHERE Id = {x}");
                ////});
                //_dbContext.Database.ExecuteSqlCommand($"UPDATE DncUser SET IsDeleted={(int)isDeleted} WHERE Id IN ({ids})");
                var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = $"UPDATE DncUser SET IsDeleted=@IsDeleted WHERE Guid IN ({parameterNames})";
                parameters.Add(new SqlParameter("@IsDeleted", (int)isDeleted));
                await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="status">用户状态</param>
        /// <param name="ids">用户ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateStatus(UserStatus status, string ids)
        {
            await using (_dbContext)
            {
                var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = $"UPDATE DncUser SET Status=@Status WHERE Guid IN ({parameterNames})";
                parameters.Add(new SqlParameter("@Status", (int)status));
                await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion
    }
}