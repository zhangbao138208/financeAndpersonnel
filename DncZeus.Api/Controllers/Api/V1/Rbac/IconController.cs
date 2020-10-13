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
using DncZeus.Api.RequestPayload.Rbac.Icon;
using DncZeus.Api.ViewModels.Rbac.DncIcon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using SqlParameter = Microsoft.Data.SqlClient.SqlParameter;

namespace DncZeus.Api.Controllers.Api.V1.Rbac
{
    /// <summary>
    /// 
    /// </summary>
    //[CustomAuthorize]
    [Route("api/v1/rbac/[controller]/[action]")]
    [ApiController]
    [CustomAuthorize]
    public class IconController : ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public IconController(DncZeusDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<IconJsonModel>>>>
            List(IconRequestPayload payload)
        {
            await using (_dbContext)
            {
                var query = _dbContext.DncIcon.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.Code.Contains(payload.Kw.Trim()));
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
                var data = list.Select(_mapper.Map<DncIcon, IconJsonModel>);
                var response = ResponseModelFactory.CreateResultInstance;
                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/v1/rbac/icon/find_list_by_kw/{kw}")]
        public async Task<ActionResult<KeyWordModel>> FindByKeyword(string kw)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            if (string.IsNullOrEmpty(kw))
            {
                response.SetFailed("没有查询到数据");
                return Ok(response);
            }

            await using (_dbContext)
            {

                var query = _dbContext.DncIcon.Where(x => x.Code.Contains(kw));

                var list = await query.ToListAsync();
                var data = list.Select(x => new { x.Code, x.Color, x.Size });

                response.SetData(data);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建图标
        /// </summary>
        /// <param name="model">图标视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Create(IconCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Code.Trim().Length <= 0)
            {
                response.SetFailed("请输入图标名称");
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (await _dbContext.DncIcon.CountAsync(x => x.Code == model.Code) > 0)
                {
                    response.SetFailed("图标已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<IconCreateViewModel, DncIcon>(model);
                entity.CreatedOn = DateTime.Now;
                entity.CreatedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;
                await _dbContext.DncIcon.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑图标
        /// </summary>
        /// <param name="id">图标ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel<IconCreateViewModel>>> Edit(int id)
        {
            await using (_dbContext)
            {
                var entity =await _dbContext.DncIcon.FirstOrDefaultAsync(x => x.Id == id);
                var response = ResponseModelFactory.CreateInstance;
                response.SetData(_mapper.Map<DncIcon, IconCreateViewModel>(entity));
                return Ok(response);
            }
        }

        /// <summary>
        /// 保存编辑后的图标信息
        /// </summary>
        /// <param name="model">图标视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Edit(IconCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Code.Trim().Length <= 0)
            {
                response.SetFailed("请输入图标名称");
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (_dbContext.DncIcon.Count(x => x.Code == model.Code && x.Id != model.Id) > 0)
                {
                    response.SetFailed("图标已存在");
                    return Ok(response);
                }
                var entity = _dbContext.DncIcon.FirstOrDefault(x => x.Id == model.Id);
                if (entity != null)
                {
                    entity.Code = model.Code;
                    entity.Color = model.Color;
                    entity.Custom = model.Custom;
                    entity.Size = model.Size;
                    entity.IsDeleted = model.IsDeleted;
                    entity.ModifiedByUserGuid = AuthContextService.CurrentUser.Guid;
                    entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;
                    entity.ModifiedOn = DateTime.Now;
                    entity.Status = model.Status;
                    entity.Description = model.Description;
                }

                await _dbContext.SaveChangesAsync();
                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除图标
        /// </summary>
        /// <param name="ids">图标ID,多个以逗号分隔</param>
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
        /// 恢复图标
        /// </summary>
        /// <param name="ids">图标ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpPost("{ids}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Recover(string ids)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }
            response = await UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
            return Ok(response);
        }

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">图标ID,多个以逗号分隔</param>
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
                    }
                    else
                    {
                        response = await UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
                    }
                    break;
                case "recover":
                    response = await UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
                    break;
                case "forbidden":
                    if (ConfigurationManager.AppSettings.IsTrialVersion)
                    {
                        response.SetIsTrial();
                    }
                    else
                    {
                        response = await UpdateStatus(UserStatus.Forbidden, ids);
                    }
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
        /// 创建图标
        /// </summary>
        /// <param name="model">多行图标视图</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Import(IconImportViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }
            if (model.Icons.Trim().Length <= 0)
            {
                response.SetFailed("没有可用的图标");
                return Ok(response);
            }
            var models = model.Icons.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => new DncIcon
            {
                Code = x.Trim(),
                CreatedByUserGuid = AuthContextService.CurrentUser.Guid,
                CreatedOn = DateTime.Now,
                CreatedByUserName = "超级管理员"
            });
            await using (_dbContext)
            {
                await _dbContext.DncIcon.AddRangeAsync(models);
                await _dbContext.SaveChangesAsync();
                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除图标
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">图标ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids)
        {
            await using (_dbContext)
            {
                var parameters = ids.Split(",").Select((id, index) => new Microsoft.Data.SqlClient.SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = $"UPDATE DncIcon SET IsDeleted=@IsDeleted WHERE Id IN ({parameterNames})";
                parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@IsDeleted", (int)isDeleted));
                await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        /// <summary>
        /// 删除图标
        /// </summary>
        /// <param name="status">图标状态</param>
        /// <param name="ids">图标ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateStatus(UserStatus status, string ids)
        {
            await using (_dbContext)
            {
                var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = $"UPDATE DncIcon SET Status=@Status WHERE Id IN ({parameterNames})";
                parameters.Add(new SqlParameter("@Status", (int)status));
                await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
    }
}