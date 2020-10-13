using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.Workflow.List;
using DncZeus.Api.Services;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.Workflow.List;
using DncZeus.Api.ViewModels.Workflow.Receiver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Controllers.Api.V1.Workflow
{
    [Route("api/v1/workflow/[controller]/[action]")]
    [ApiController]
    [CustomAuthorize]
    public class WListController : ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly DictionaryService _dictionaryService;
        private readonly TelegramService _telegramService;

        public WListController(DncZeusDbContext dbContext, IMapper mapper, 
            DictionaryService dictionaryService, TelegramService telegramService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _dictionaryService = dictionaryService;
            _telegramService = telegramService;
        }
        /// <summary>
        /// 审批工作列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<WorkflowListJsonModel>>>> List(WorkflowListRequestPayload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            await using (_dbContext)
            {
                var query = (from ls in _dbContext.WorkflowList
                             join de in _dbContext.UserDepartment on ls.DepartmentCode equals de.Code
                             into t1
                             from de in t1.DefaultIfEmpty()
                             join tl in _dbContext.WorkflowTemplate on ls.TemplateCode equals tl.Code
                             into t2
                             from tl in t2.DefaultIfEmpty()

                             join user in _dbContext.DncUser on ls.User equals user.Guid
                             into t3
                             from user in t3.DefaultIfEmpty()

                             join step1 in _dbContext.WorkflowStep on ls.CurrentStepCode equals step1.Code
                             into t4
                             from step1 in t4.DefaultIfEmpty()

                             join step2 in _dbContext.WorkflowStep on ls.NextStepCode equals step2.Code
                             into t5
                             from step2 in t5.DefaultIfEmpty()
                             select new WorkflowListJsonModel
                             {
                                 Code = ls.Code,
                                 DepartmentCode = ls.DepartmentCode,
                                 Description = ls.Description,
                                 EndDate = ls.EndDate,
                                 StartDate = ls.StartDate,
                                 CurrentStepCode = ls.CurrentStepCode,
                                 NextStepCode = ls.NextStepCode,
                                 NotifyUser = ls.NotifyUser,
                                 Number = ls.Number,
                                 Status = ls.Status,
                                 TemplateCode = ls.TemplateCode,
                                 Title = ls.Title,
                                 Type = ls.Type,
                                 User = ls.User,
                                 DepartmentName = de.Name,
                                 TemplateName = tl.Name,
                                 UserName = user.DisplayName,
                                 CurrentStepName = step1.Title,
                                 NextStepName = step2.Title,
                             });
                query = query.Where(q=>q.User== AuthContextService.CurrentUser.Guid);
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.Title.Contains(payload.Kw.Trim()) ||
                   x.Code.Contains(payload.Kw.Trim()));
                }
                var list = await query.Paged(payload.CurrentPage, payload.PageSize).ToListAsync();
                list.ForEach(r=> {
                    var dic = _dictionaryService.GetSYSDictionary("workflow_list_type", r.Type);
                    r.TypeName = dic?.Name;
                    var dic1 = _dictionaryService.GetSYSDictionary("workflow_list_status", r.Status);
                    r.StatusName = dic1?.Name;
                });
                var totalCount = query.Count();
                var data = list;

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建审批工作
        /// </summary>
        /// <param name="model">审批工作视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Create(WorkflowListCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Title.Trim().Length <= 0)
            {
                response.SetFailed("请输入审批工作名称");
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (_dbContext.DncRole.Count(x => x.Name == model.Title) > 0)
                {
                    response.SetFailed("审批工作已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<WorkflowListCreateViewModel, WorkflowList>(model);
                entity.User= AuthContextService.CurrentUser.Guid;
                entity.StartDate = DateTime.Now;
                entity.Status = "0";
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                //进行下一步审批
                if (!string.IsNullOrEmpty(model.NextStepCode)&& !string.IsNullOrEmpty(model.Approver))
                {
                    //进行中
                    entity.Status = "1";
                    entity.CurrentStepCode = model.NextStepCode;
                    var step = await _dbContext.WorkflowStep.FindAsync(model.NextStepCode);
                    if (step!=null)
                    {
                        var steps= await _dbContext.WorkflowStep.
                            Where(s=>s.TemplateCode==step.TemplateCode).OrderBy(o => o.SortID).ToListAsync();
                        if (steps.Count>int.Parse(step.SortID))
                        {
                            entity.NextStepCode = steps[int.Parse(step.SortID)]?.Code;
                        }
                    }
                    var receivers = new List<WorkflowReceiver>();
                    foreach (var user in model.Approver.Split(','))
                    {
                        var u = Guid.Parse(user);
                        //发送审批到下一个人
                        var receiver = WorkflowReceiverFactory.CreateInstance;
                        receiver.Type = entity.Type;
                        receiver.User = u;
                        receiver.WorkflowCode = entity.Code;
                        receiver.CreateUser = entity.User;
                        receiver.Description = entity.Description;
                        receiver.StepCode = entity.CurrentStepCode;
                        receiver.TemplateCode = step?.TemplateCode;
                        receivers.Add(receiver);
                        //发送纸飞机通知
                        var toDoUser = await _dbContext.DncUser.FindAsync(entity.User);
                        var rUser= await _dbContext.DncUser.FindAsync(u);
                        await _telegramService.SendTextMessageAsync(rUser.TelegramChatId, 
                            $"请审核用户【{toDoUser.DisplayName}】{entity.Title}",rUser.TelegramBotToken);
                    }
                   

                    await _dbContext.WorkflowReceiver.AddRangeAsync(receivers);

                }
                
                await _dbContext.WorkflowList.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑审批工作
        /// </summary>
        /// <param name="code">审批工作惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel<WorkflowListCreateViewModel>>> Edit(string code)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.WorkflowList.FirstOrDefaultAsync(x => x.Code == code);
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<WorkflowList, WorkflowListCreateViewModel>(entity);

                var receivers = await _dbContext.WorkflowReceiver.
                   Where(w => w.WorkflowCode == entity.Code).ToListAsync();
                //计算当前审批流程
                resEntity.CurrentStepCode = (await _dbContext.WorkflowReceiver.Where(l => 
                l.WorkflowCode == resEntity.Code &&
                 !l.IsCheck).FirstOrDefaultAsync())?.StepCode;
                var notes = new List<Note>();
                foreach (var receiver in receivers)
                {
                    var note = new Note();
                    var step = await _dbContext.WorkflowStep.FindAsync(receiver.StepCode);
                    note.NodeName = step?.Title;
                    var dic = await _dictionaryService.GetSYSDictionaryAsync("workflow_receiver_status", receiver.Status);
                    note.StatusName =  dic?.Name;
                    note.Status = receiver.Status;
                    var user = await _dbContext.DncUser.FindAsync(receiver.User);
                    note.UserName = user?.DisplayName;
                    note.Opinion = receiver.Note;
                    note.NodeDate = receiver.CheckDate ==
                        DateTime.MinValue ? "" : receiver.CheckDate.ToString("yyyy-MM-dd");
                    notes.Add(note);
                }
                resEntity.Notes = notes;

                response.SetData(resEntity);
                return Ok(response);
            }
        }


        /// <summary>
        /// 保存编辑后的审批工作信息
        /// </summary>
        /// <param name="model">审批工作视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Edit(WorkflowListCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (model.Status!="0")
                {
                    response.SetFailed("改工作不在【新工作】不能修改");
                    return Ok(response);
                }

                var entity = _mapper.Map<WorkflowListCreateViewModel, WorkflowList>(model);
                //进行下一步审批
                if (!string.IsNullOrEmpty(model.NextStepCode) && !string.IsNullOrEmpty(model.Approver))
                {
                    //进行中
                    entity.Status = "1";
                    //进行中
                    entity.Status = "1";
                    entity.CurrentStepCode = model.NextStepCode;
                    var step = await _dbContext.WorkflowStep.FindAsync(model.NextStepCode);
                    if (step != null)
                    {
                        var steps = await _dbContext.WorkflowStep.
                            Where(s => s.TemplateCode == step.TemplateCode).OrderBy(o => o.SortID).ToListAsync();
                        if (steps.Count > int.Parse(step.SortID))
                        {
                            entity.NextStepCode = steps[int.Parse(step.SortID)]?.Code;
                        }
                    }
                    var receivers = new List<WorkflowReceiver>();
                    foreach (var user in model.Approver.Split(','))
                    {
                        var u = Guid.Parse(user);
                        //发送审批到下一个人
                        var receiver = WorkflowReceiverFactory.CreateInstance;
                        receiver.Type = entity.Type;
                        receiver.User = u;
                        receiver.Description = entity.Description;
                        receiver.WorkflowCode = entity.Code;
                        receiver.CreateUser = entity.User;
                        receiver.StepCode = entity.CurrentStepCode;
                        receiver.TemplateCode = step?.TemplateCode;
                        receivers.Add(receiver);
                        //发送纸飞机通知
                        var toDoUser = await _dbContext.DncUser.FindAsync(entity.User);
                        var rUser = await _dbContext.DncUser.FindAsync(u);
                        await _telegramService.SendTextMessageAsync(rUser.TelegramChatId,
                            $"请审核用户【{toDoUser.DisplayName}】{entity.Title}", rUser.TelegramBotToken);
                    }

                    await _dbContext.WorkflowReceiver.AddRangeAsync(receivers);
                }
                _dbContext.Entry(entity).State = EntityState.Modified;
                 await  _dbContext.SaveChangesAsync();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除审批工作
        /// </summary>
        /// <param name="ids">审批工作code,多个以逗号分隔</param>
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
            if (string.IsNullOrWhiteSpace(ids))
            {
                response.SetFailed("请选择删除项");
                return Ok(response);
            }
            if (await _dbContext.WorkflowList.Where(w=>w.Status!="0"&&ids.Contains(w.Code)).AnyAsync())
            {
                response.SetFailed("请选择状态为新工作的项");
                return Ok(response);
            }

            await using (_dbContext)
            {
                _dbContext.WorkflowList.RemoveRange(_dbContext.WorkflowList
                    .Where(w => ids.Contains(w.Code)).ToList()
                    );
              await  _dbContext.SaveChangesAsync();

            }

            return Ok(response);
        }


        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">审批工作ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [Obsolete]
        public async Task<ActionResult<ResponseModel>> Batch(string command, string ids)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (string.IsNullOrWhiteSpace(ids))
            {
                response.SetFailed("请选操作项");
                return Ok(response);
            }
            switch (command)
            {
                case "delete":
                    if (ConfigurationManager.AppSettings.IsTrialVersion)
                    {
                        response.SetIsTrial();
                        return Ok(response);
                    }
                    if (await _dbContext.WorkflowList.Where(w => w.Status != "0" 
                    && ids.Contains(w.Code)).AnyAsync())
                    {
                        response.SetFailed("请选择状态为新工作的项");
                        return Ok(response);
                    }
                    response = await UpdateIsDelete(ids);
                    return response;

                //case "recover":
                //    response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
                //    break;
                //case "forbidden":
                //    if (ConfigurationManager.AppSettings.IsTrialVersion)
                //    {
                //        response.SetIsTrial();
                //        return Ok(response);
                //    }
                //    response = UpdateStatus(UserStatus.Forbidden, ids);
                //    break;
                //case "normal":
                //    response = UpdateStatus(UserStatus.Normal, ids);
                //   break;
                default:
                    break;
            }
            return Ok(response);
        }

        #region 私有方法

        /// <summary>
        /// 删除财务账号
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Obsolete]
        private async Task<ResponseModel> UpdateIsDelete(string ids)
        {
            await using (_dbContext)
            {
                var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = $"DELETE WorkflowList  WHERE Code IN ({parameterNames})";
                await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion
    }
}
