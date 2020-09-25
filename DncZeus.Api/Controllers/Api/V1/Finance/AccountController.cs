using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Entities.Enums;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.Finance.Account;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.Finance.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DncZeus.Api.Controllers.Api.V1.Finance
{
    [Route("api/v1/finance/[controller]/[action]")]
    [ApiController]
    [CustomAuthorize]
    public class AccountController:ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;

        public AccountController(DncZeusDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        /// <summary>
        /// 财务账号列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ResponseResultModel<IEnumerable<AccountJsonModel>>> List(AccountRequestPayload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            using (_dbContext)
            {
                var query = _dbContext.FinanceAccount.AsQueryable();
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
                var list = query.Paged(payload.CurrentPage, payload.PageSize).ToList();
                var totalCount = query.Count();
                var data = list.Select(_mapper.Map<FinanceAccount, AccountJsonModel>);

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建财务账号
        /// </summary>
        /// <param name="model">财务账号视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public ActionResult<ResponseModel> Create(AccountCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Name.Trim().Length <= 0)
            {
                response.SetFailed("请输入财务账号名称");
                return Ok(response);
            }
            using (_dbContext)
            {
                if (_dbContext.DncRole.Count(x => x.Name == model.Name) > 0)
                {
                    response.SetFailed("财务账号已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<AccountCreateViewModel, FinanceAccount>(model);

                entity.CreatedOn = DateTime.Now;
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                entity.CreatedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;

                _dbContext.FinanceAccount.Add(entity);
                _dbContext.SaveChanges();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑财务账号
        /// </summary>
        /// <param name="code">财务账号惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public ActionResult<ResponseModel<AccountCreateViewModel>> Edit(string code)
        {
            using (_dbContext)
            {
                var entity = _dbContext.FinanceAccount.FirstOrDefault(x => x.Code == code);
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<FinanceAccount, AccountCreateViewModel>(entity);
                response.SetData(resEntity);
                return Ok(response);
            }
        }


        /// <summary>
        /// 保存编辑后的财务账号信息
        /// </summary>
        /// <param name="model">角色视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public ActionResult<ResponseModel> Edit(AccountCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }
            using (_dbContext)
            {
                if (_dbContext.FinanceAccount.Count(x => x.Name == model.Name && x.Code != model.Code) > 0)
                {
                    response.SetFailed("财务账号已存在");
                    return Ok(response);
                }

                var entity = _dbContext.FinanceAccount.Find(model.Code);
                entity.Name = model.Name;
                entity.Status = model.Status;
                entity.Account = model.Account;
                entity.Type = model.Type;
                entity.Holder = model.Holder;
                entity.Description = model.Description;

                entity.ModifiedOn = DateTime.Now;
                entity.ModifiedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;

                _dbContext.Entry(entity).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除财务账号
        /// </summary>
        /// <param name="ids">财务账号code,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpDelete("{ids}")]
        [ProducesResponseType(200)]
        public ActionResult<ResponseModel> Delete(string ids)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }
            response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
            return Ok(response);
        }

        /// <summary>
        /// 恢复财务账号
        /// </summary>
        /// <param name="ids">财务账号ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpPost("{ids}")]
        [ProducesResponseType(200)]
        public ActionResult<ResponseModel> Recover(string ids)
        {
            var response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
            return Ok(response);
        }
        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">财务账号ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public ActionResult<ResponseModel> Batch(string command, string ids)
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
                    response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
                    break;
                case "recover":
                    response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
                    break;
                case "forbidden":
                    if (ConfigurationManager.AppSettings.IsTrialVersion)
                    {
                        response.SetIsTrial();
                        return Ok(response);
                    }
                    response = UpdateStatus(UserStatus.Forbidden, ids);
                    break;
                case "normal":
                    response = UpdateStatus(UserStatus.Normal, ids);
                    break;
                default:
                    break;
            }
            return Ok(response);
        }
        /// <summary>
        /// 查询所有财务账号列表(只包含主要的字段信息:name,code)
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/v1/finance/account/find_simple_list")]
        public ActionResult<ResponseResultModel<IEnumerable<SimpleModel>>> FindSimpleList()
        {
            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext)
            {
                var roles = _dbContext.FinanceAccount.
                    Where(x => x.IsDeleted == CommonEnum.IsDeleted.No && x.Status == CommonEnum.Status.Normal).
                    Select(x => new { x.Name, x.Code }).ToList();
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
        private ResponseModel UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids)
        {
            using (_dbContext)
            {
                var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = string.Format("UPDATE FinanceAccount SET IsDeleted=@IsDeleted WHERE Code IN ({0})", parameterNames);
                parameters.Add(new SqlParameter("@IsDeleted", (int)isDeleted));
                _dbContext.Database.ExecuteSqlCommand(sql, parameters);
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
        private ResponseModel UpdateStatus(UserStatus status, string ids)
        {
            using (_dbContext)
            {
                var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = string.Format("UPDATE FinanceAccount SET Status=@Status WHERE Code IN ({0})", parameterNames);
                parameters.Add(new SqlParameter("@Status", (int)status));
                _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion
    }
}
