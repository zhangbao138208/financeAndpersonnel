using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Entities.Enums;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.Resume;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.Refuse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Controllers.Api.V1.Resume
{
    [Route("api/v1/Resume/[controller]/[action]")]
    [ApiController]
    [CustomAuthorize]
    public class ResumeInfoController:ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;

        public ResumeInfoController(DncZeusDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        /// <summary>
        /// 简历
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<ResumeJsonModel>>>>
            List(ResumeRequestPload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            await using (_dbContext)
            {
                var query = (from resume in _dbContext.ResumeInfo
                join position in _dbContext.UserPosition on resume.PositionCode equals position.Code
                into t1
                from position in t1.DefaultIfEmpty()
                join department in _dbContext.UserDepartment on resume.DepartmentCode equals department.Code
                into t2
                from department in t2.DefaultIfEmpty()
                select new ResumeJsonModel{
                    RealName=resume.RealName,
                    Age=resume.Age,
                    PositionCode=resume.PositionCode,
                    PositionName=position.Name,
                    DepartmentCode=resume.DepartmentCode,
                    DepartmentName=department.Name,
                    IsDeleted=resume.IsDeleted,
                    LevelID=resume.LevelID,
                    Email=resume.Email,
                    Mobile=resume.Mobile,
                    Code=resume.Code,
                    Years=resume.Years,
                    Status=resume.Status,
                    CreatedOn=resume.CreatedOn.ToString(),
                    CreatedByUserGuid=resume.CreatedByUserGuid,
                    CreatedByUserName=resume.CreatedByUserName,
                    ModifiedOn = resume.ModifiedOn.ToString(),
                    ModifiedByUserGuid = resume.ModifiedByUserGuid,
                    ModifiedByUserName = resume.ModifiedByUserName
                });
                //var query = _dbContext.ResumeInfo.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.RealName.Contains(payload.Kw.Trim()) || x.Code.Contains(payload.Kw.Trim()));
                }
                if (payload.IsDeleted > CommonEnum.IsDeleted.All)
                {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (payload.Status > CommonEnum.Status.All)
                {
                    query = query.Where(x => x.Status == payload.Status);
                }
                var list = await query.Paged(payload.CurrentPage, payload.PageSize).
                    OrderBy(r => r.LevelID).ToListAsync();
                var totalCount = await query.CountAsync();
                //var data = list.Select(_mapper.Map<ResumeInfo, ResumeJsonModel>).ToList();
                var data = list.ToList();
                //data.ForEach(d=> {
                //    d.PositionName = _dbContext.UserPosition.Find(d.PositionCode)?.Name;
                //    d.DepartmentName = _dbContext.UserDepartment.Find(d.DepartmentCode)?.Name;
                //});
                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建简历
        /// </summary>
        /// <param name="model">简历视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Create(ResumeCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.RealName.Trim().Length <= 0)
            {
                response.SetFailed("请输入简历名称");
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (await _dbContext.ResumeInfo.
                    CountAsync(x => x.RealName == model.RealName) > 0)
                {
                    response.SetFailed("简历已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<ResumeCreateViewModel, ResumeInfo>(model);
                entity.CreatedOn = DateTime.Now;
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                entity.CreatedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;

                await _dbContext.ResumeInfo.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑简历
        /// </summary>
        /// <param name="code">简历惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel<ResumeCreateViewModel>>> Edit(string code)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.ResumeInfo.FindAsync(code);
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<ResumeInfo, ResumeCreateViewModel>(entity);
                response.SetData(resEntity);
                return Ok(response);
            }
        }


        /// <summary>
        /// 保存编辑后的简历信息
        /// </summary>
        /// <param name="model">角色视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Edit(ResumeCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (await _dbContext.ResumeInfo.CountAsync(x => x.RealName == model.RealName && 
                                                     x.Code != model.Code) > 0)
                {
                    response.SetFailed("简历已存在");
                    return Ok(response);
                }

                var entity = _mapper.Map<ResumeCreateViewModel, ResumeInfo>(model);

                

                entity.ModifiedOn = DateTime.Now;
                entity.ModifiedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;

                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除简历
        /// </summary>
        /// <param name="ids">简历code,多个以逗号分隔</param>
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
        /// 恢复简历
        /// </summary>
        /// <param name="ids">简历ID,多个以逗号分隔</param>
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
        /// <param name="ids">简历ID,多个以逗号分隔</param>
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
                // var sql = $"UPDATE ResumeInfo SET IsDeleted=@IsDeleted WHERE Code IN ({parameterNames})";
                // parameters.Add(new SqlParameter("@IsDeleted", (int)isDeleted));
                // await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE ResumeInfo SET IsDeleted={(int)isDeleted} WHERE Code IN ({formatIds})";
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
                // var sql = $"UPDATE ResumeInfo SET Status=@Status WHERE Code IN ({parameterNames})";
                // parameters.Add(new SqlParameter("@Status", (int)status));
                // await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE ResumeInfo SET Status={(int)status} WHERE Code IN ({formatIds})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion
    }
}
