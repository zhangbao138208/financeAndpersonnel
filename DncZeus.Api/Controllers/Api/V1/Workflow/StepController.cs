using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Entities.Enums;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.Workflow.Step;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.Workflow.Step;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Controllers.Api.V1.Workflow
{
    [ApiController]
    // [ServiceFilter(typeof(CustomAuthorizeAttribute))]
    [ServiceFilter(typeof(CustomAuthorizeAttribute))]
    [Route("api/v1/Workflow/[controller]/[action]")]
    public class StepController : ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;

        public StepController(DncZeusDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;

        }

        /// <summary>
        /// 步骤列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<StepJsonModel>>>>
            List(StepRequestPaload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            await using (_dbContext)
            {
                var query = (from l in _dbContext.WorkflowStep
                             join tem in _dbContext.WorkflowTemplate on
                             l.TemplateCode equals tem.Code
                             into t1
                             from tem in t1.DefaultIfEmpty()

                             select new StepJsonModel
                             {
                                 Code = l.Code,
                                 IsCounterSign = l.IsCounterSign,
                                 SortID = l.SortID,
                                 TemplateCode = l.TemplateCode,
                                 TemplateName = tem.Name,
                                 UserList = l.UserList,
                                 Title = l.Title,
                                 Status = l.Status,
                                 IsManual = l.IsManual,
                                 IsDeleted = l.IsDeleted,
                                 CreatedOn = l.CreatedOn.ToString(),
                                 CreatedByUserGuid = l.CreatedByUserGuid,
                                 CreatedByUserName = l.CreatedByUserName,
                                 ModifiedOn = l.ModifiedOn.ToString(),
                                 ModifiedByUserGuid = l.ModifiedByUserGuid,
                                 ModifiedByUserName = l.ModifiedByUserName
                             });
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.Title.Contains(payload.Kw.Trim()) || x.Code.Contains(payload.Kw.Trim()));
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
                    OrderBy(r => r.SortID).ToListAsync();
                var totalCount = query.Count();
                var data = list;

                foreach (var item in data)
                {
                    var users = _dbContext.DncUser.
                        Where(u => item.UserList.Contains(u.Guid.ToString(),StringComparison.OrdinalIgnoreCase)).
                        Select(r => r.DisplayName).ToArray();
                    item.UserListName = string.Join('|', users);
                }

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建步骤
        /// </summary>
        /// <param name="model">步骤视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Create(StepCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Title.Trim().Length <= 0)
            {
                response.SetFailed("请输入步骤名称");
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (await _dbContext.WorkflowStep.
                    CountAsync(x => (x.Title == model.Title&& x.TemplateCode==model.TemplateCode)||
                                    (x.TemplateCode==model.TemplateCode && x.SortID==model.SortID)) > 0)
                {
                    response.SetFailed("步骤已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<StepCreateViewModel, WorkflowStep>(model);
                entity.UserList = string.Join('|', model.UserList);
                entity.CreatedOn = DateTime.Now;
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                entity.CreatedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;

                await _dbContext.WorkflowStep.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑步骤
        /// </summary>
        /// <param name="code">步骤惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")] 
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel<StepCreateViewModel>>> Edit(string code)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.WorkflowStep.
                    FirstOrDefaultAsync(x => x.Code == code);
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<WorkflowStep, StepCreateViewModel>(entity);
                if (entity != null) resEntity.UserList = entity.UserList.Split('|').ToList();
                response.SetData(resEntity);
                return Ok(response);
            }
        }


        /// <summary>
        /// 保存编辑后的步骤信息
        /// </summary>
        /// <param name="model">角色视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Edit(StepCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (await _dbContext.WorkflowStep.
                    CountAsync(x => (x.Title == model.Title &&
                                     x.TemplateCode==model.TemplateCode && x.Code != model.Code) ||(
                   x.SortID==model.SortID && x.TemplateCode==model.TemplateCode && x.Code != model.Code)) > 0)
                {
                    response.SetFailed("步骤已存在");
                    return Ok(response);
                }

                var entity = await _dbContext.WorkflowStep.FindAsync(model.Code);

                entity.Title = model.Title;
                entity.SortID = model.SortID;
                entity.TemplateCode = model.TemplateCode;
                entity.UserList = string.Join('|', model.UserList);
                entity.Status = model.Status;
                entity.IsCounterSign = model.IsCounterSign;
                entity.IsManual = model.IsManual;

                entity.ModifiedOn = DateTime.Now;
                entity.ModifiedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;

                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除步骤
        /// </summary>
        /// <param name="ids">步骤code,多个以逗号分隔</param>
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
            response =await UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
            return Ok(response);
        }

        /// <summary>
        /// 恢复步骤
        /// </summary>
        /// <param name="ids">步骤ID,多个以逗号分隔</param>
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
        /// <param name="ids">步骤ID,多个以逗号分隔</param>
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
        /// 查询所有步骤列表(只包含主要的字段信息:name,code)
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/v1/Workflow/Step/find_simple_list")]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<StepSimpleJsomModel>>>>
            FindSimpleListAsync(string templateCode)
        {
            var response = ResponseModelFactory.CreateInstance;
            await using (_dbContext)
            {
                var query = _dbContext.WorkflowStep.
                    Where(x => x.IsDeleted == CommonEnum.IsDeleted.No && x.Status == CommonEnum.Status.Normal);
                if (!string.IsNullOrEmpty(templateCode))
                {
                    query = query.Where(q => q.TemplateCode == templateCode);
                }

                var steps = await query.OrderBy(r => r.SortID).
                    Select(x => new StepSimpleJsomModel
                    {
                        Code = x.Code,
                        Title = x.Title,
                        IsCounterSign = x.IsCounterSign,
                        UserList = x.UserList
                    }).ToListAsync();
                foreach (var s in steps)
                {
                    s.Users = s.UserList.Split('|').ToArray();
                    s.UsersName = await _dbContext.DncUser.
                    Where(us => s.UserList.Contains(us.Guid.ToString(),StringComparison.OrdinalIgnoreCase)).
                    Select(user => user.DisplayName).ToArrayAsync();
                }
                //这种异步写法有误
                //steps.ForEach( async(s)=> {
                //    s.Users = s.UserList.Split('|').ToArray();
                //    s.UsersName = await _dbContext.DncUser.
                //    Where(us => s.UserList.Contains(us.Guid.ToString())).
                //    Select(s => s.DisplayName).ToArrayAsync();
                //});
                response.SetData(steps);
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
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE WorkflowStep SET IsDeleted={(int)isDeleted} WHERE Code IN ({formatIds})";
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
        private  async Task<ResponseModel> UpdateStatus(UserStatus status, string ids)
        {
            await using (_dbContext)
            {
                // var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                // var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                // var sql = string.Format("UPDATE WorkflowStep SET Status=@Status WHERE Code IN ({0})", parameterNames);
                // parameters.Add(new SqlParameter("@Status", (int)status));
                // await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE WorkflowStep SET Status={(int)status} WHERE Code IN ({formatIds})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion
    }
}
