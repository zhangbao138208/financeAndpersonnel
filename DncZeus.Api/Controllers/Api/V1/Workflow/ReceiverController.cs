using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.Workflow.Receiver;
using DncZeus.Api.Services;
using DncZeus.Api.ViewModels.Workflow.Receiver;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Controllers.Api.V1.Workflow
{
    [Route("api/v1/workflow/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(CustomAuthorizeAttribute))]
    public class ReceiverController : ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly DictionaryService _dictionaryService;
        private readonly TelegramService _telegramService;

        public ReceiverController(DncZeusDbContext dbContext, IMapper mapper,
            DictionaryService dictionaryService, TelegramService telegramService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _dictionaryService = dictionaryService;
            _telegramService = telegramService;
        }

        /// <summary>
        /// 我的审批列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<ReceiverJsonModel>>>>
            List(ReceiverRequestPaload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            await using (_dbContext)
            {
                var query = (from ls in _dbContext.WorkflowReceiver
                    join w in _dbContext.WorkflowList on ls.WorkflowCode equals w.Code
                        into q1
                    from w in q1.DefaultIfEmpty()
                    join tl in _dbContext.WorkflowTemplate on ls.TemplateCode equals tl.Code
                        into t2
                    from tl in t2.DefaultIfEmpty()
                    join user in _dbContext.DncUser on ls.User equals user.Guid
                        into t3
                    from user in t3.DefaultIfEmpty()
                    join step1 in _dbContext.WorkflowStep on ls.StepCode equals step1.Code
                        into t4
                    from step1 in t4.DefaultIfEmpty()
                    join u1 in _dbContext.DncUser on ls.User equals u1.Guid
                        into t5
                    from u1 in t5.DefaultIfEmpty()
                    join u2 in _dbContext.DncUser on ls.CreateUser equals u2.Guid
                        into t6
                    from u2 in t6.DefaultIfEmpty()
                    select new ReceiverJsonModel
                    {
                        Id = ls.Id,
                        CheckDate = ls.CheckDate,
                        CreateDate = ls.CreateDate,
                        CreateUser = ls.CreateUser,
                        Note = ls.Note,
                        StepCode = ls.StepCode,
                        WorkflowCode = ls.WorkflowCode,
                        IsCheck = ls.IsCheck,
                        OldStatus = ls.Status,
                        Status = w.EndDate < DateTime.Now && !ls.IsCheck ? "-1" : ls.Status,
                        TemplateCode = ls.TemplateCode,
                        ListType = ls.Type,
                        User = ls.User,
                        TemplateName = tl.Name,
                        Additions = ls.Additions,
                        UserName = user.DisplayName,
                        WorkflowName = w.Title,
                        StepName = step1.Title,
                    });
                ;
                ;
                if (AuthContextService.CurrentUser.UserType != UserType.SuperAdministrator)
                {
                    query = query.Where(q => q.User == AuthContextService.CurrentUser.Guid);
                }

                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.WorkflowName.Contains(payload.Kw.Trim()) ||
                                             x.Note.Contains(payload.Kw.Trim()));
                }

                if (!string.IsNullOrEmpty(payload.Status) && payload.Status != "all")
                {
                    query = query.Where(x => x.Status == payload.Status.Trim());
                }

                var list = await query.OrderByDescending(o=>o.CreateDate).
                    Paged(payload.CurrentPage, payload.PageSize).ToListAsync();
                foreach (var r in list)
                {
                    var dic = await _dictionaryService.GetSYSDictionaryAsync("workflow_list_type", r.ListType);
                    r.ListTypeName = dic?.Name;
                    var dic1 = await _dictionaryService.GetSYSDictionaryAsync("workflow_receiver_status", r.Status);
                    r.StatusName = dic1?.Name;
                    //只要会签中有一个不同意都不能编辑 或签只能选一个人所以不会出现这种情况
                    r.CanEdit = !(await _dbContext.WorkflowReceiver.AnyAsync(a =>
                        a.Status == "2" && a.WorkflowCode == r.WorkflowCode));
                }

                var totalCount = query.Count();
                var data = list;
                //更新已过期
                var arr = data
                    .Where(d => d.Status == "-1"
                                && d.OldStatus != "-1").Select(s => s.Id).ToArray();
                if (arr.Length > 0)
                {
                    await UpdateStatus(arr);
                }

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 审批信息
        /// </summary>
        /// <param name="code">审批工作惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel<ReceiverEditViewModel>>> Edit(string code)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.WorkflowReceiver.FindAsync(int.Parse(code));
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<WorkflowReceiver, ReceiverEditViewModel>(entity);
                var workflow = await _dbContext.WorkflowList.FindAsync(entity.WorkflowCode);
                resEntity.WorkflowName = workflow?.Title;
                resEntity.ListType = workflow?.Type;
                resEntity.EndDate = workflow?.EndDate.ToString("yyyy-MM-dd HH:MM:ss");

                resEntity.CanChoose = (await _dbContext.WorkflowReceiver.CountAsync(r =>
                    r.WorkflowCode == entity.WorkflowCode && r.StepCode == entity.StepCode &&
                    !r.IsCheck)) == 1;
                //查找下一步
                var currStep = await _dbContext.WorkflowStep.FindAsync(entity.StepCode);
                var nextStep =
                    await _dbContext.WorkflowStep.FirstOrDefaultAsync(n =>
                        n.SortID == (int.Parse(currStep.SortID) + 1).ToString());

                resEntity.NextStepCode = nextStep?.Code;
                resEntity.NextStepName = nextStep?.Title;

                if (nextStep == null)
                {
                    resEntity.CanChoose = false;
                }

                //下拉列表数据
                if (nextStep != null)
                {
                    resEntity.IsCounterSign = nextStep.IsCounterSign;
                    var users = await _dbContext.DncUser.Where(u => nextStep.UserList.Contains(u.Guid.ToString(),
                        StringComparison.OrdinalIgnoreCase)).Select(s => new NextUser
                    {
                        User = s.Guid,
                        UserName = s.DisplayName,
                    }).ToListAsync();
                    resEntity.NextUsers = users;
                }

                //审批状态
                var dic = await _dictionaryService.GetSYSDictionaryAsync("workflow_receiver_status");
                var statues = dic.Select(d => new ReceiverStatus
                {
                    Name = d.Name,
                    Vaue = d.Value,
                }).ToList();
                resEntity.Statuses = statues;

                response.SetData(resEntity);
                return Ok(response);
            }
        }


        /// <summary>
        /// 审批完成
        /// </summary>
        /// <param name="model">我的审批视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Edit(ReceiverCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (model.IsCheck)
                {
                    response.SetFailed("改审批已完成");
                    return Ok(response);
                }

                if (!model.IsCheck && model.Status == -1)
                {
                    response.SetFailed("改审批已过期");
                    return Ok(response);
                }

                if (await _dbContext.WorkflowReceiver.AnyAsync(a =>
                    a.Status == "2" && a.WorkflowCode == model.WorkflowCode))
                {
                    response.SetFailed("该审批已拒绝");
                    return Ok(response);
                }


                // var entity = _mapper.Map<ReceiverCreateViewModel, WorkflowReceiver>(model);
                var entity = await _dbContext.WorkflowReceiver.FindAsync(model.Id);

                //进行下一步审批
                var canNext = (await _dbContext.WorkflowReceiver.CountAsync(r =>
                    r.WorkflowCode == entity.WorkflowCode && r.StepCode == entity.StepCode &&
                    !r.IsCheck)) == 1;
                if (!string.IsNullOrEmpty(model.NextStepCode)
                    && !string.IsNullOrEmpty(model.Approver) && model.Status == 1 && canNext)
                {
                    //var step = await _dbContext.WorkflowStep.FindAsync(model.NextStepCode);
                    //var nextStepCode = "";
                    //if (step != null)
                    //{
                    //    //var steps = await _dbContext.WorkflowStep.
                    //    //    Where(s => s.TemplateCode == step.TemplateCode).OrderBy(o=>o.SortID).ToListAsync();
                    //    //if (steps.Count > int.Parse(step.SortID))
                    //    //{
                    //    //    nextStepCode = steps[int.Parse(step.SortID)]?.Code;
                    //    //}
                    //    nextStepCode= (await _dbContext.WorkflowStep.
                    //   Where(s => s.TemplateCode == step.TemplateCode&&string.Compare(s.SortID,step.SortID)>0).OrderBy(o=>o.SortID)
                    //   .FirstOrDefaultAsync()).Code;
                    //}
                    var receivers = new List<WorkflowReceiver>();
                    foreach (var user in model.Approver.Split(','))
                    {
                        var u = Guid.Parse(user);
                        //发送审批到下一个人
                        var receiver = WorkflowReceiverFactory.CreateInstance;
                        receiver.Type = entity.Type;
                        receiver.User = u;
                        receiver.Description = entity.Description;
                        receiver.WorkflowCode = entity.WorkflowCode;
                        receiver.CreateUser = AuthContextService.CurrentUser.Guid;
                        receiver.StepCode = model.NextStepCode;
                        receiver.TemplateCode = entity.TemplateCode;
                        receiver.Additions = entity.Additions;
                        receivers.Add(receiver);
                        //发送纸飞机通知
                        var toDoUser = await _dbContext.DncUser.FindAsync(entity.User);
                        var rUser = await _dbContext.DncUser.FindAsync(u);
                        var workflowList = await _dbContext.WorkflowList.FindAsync(receiver.WorkflowCode);
                        await _telegramService.SendTextMessageAsync(rUser.TelegramChatId,
                            $"请审核用户【{toDoUser.DisplayName}】{workflowList.Title}", rUser.TelegramBotToken);
                    }

                   


                    await _dbContext.WorkflowReceiver.AddRangeAsync(receivers);
                }

                
                //如果审批状态为未通过需更新工作状态
                if (model.Status == 2)
                {
                    var work = await _dbContext.WorkflowList.FindAsync(model.WorkflowCode);
                    work.Status = "2";
                    _dbContext.Entry(work).State = EntityState.Modified;
                }

                //全部同意也需要更新工作转态
                if (model.Status == 1)
                {
                    var disagree = await _dbContext.WorkflowReceiver
                        .Where(r => r.WorkflowCode == model.WorkflowCode&&!r.IsCheck)
                        .CountAsync();
                    var step = await _dbContext.WorkflowStep.Where(s => s.TemplateCode == model.TemplateCode)
                        .OrderBy(m => m.SortID).LastOrDefaultAsync();
                    if (step.Code == model.StepCode && disagree == 1)
                    {
                        var work = await _dbContext.WorkflowList.FindAsync(model.WorkflowCode);
                        work.Status = "2";
                        _dbContext.Entry(work).State = EntityState.Modified;
                    }
                }

                //设置审批状态
                entity.Status = model.Status.ToString();
                entity.Description = model.Description;
                entity.Note = model.Note;
                //设置已审批
                entity.IsCheck = true;
                entity.CheckDate = DateTime.Now;
                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(response);
            }
        }

        /// <summary>
        /// 查看审批信息
        /// </summary>
        /// <param name="code">审批工作惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel<ReceiverReadOnlyModel>>> View(string code)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.WorkflowReceiver.FindAsync(int.Parse(code));
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<WorkflowReceiver, ReceiverReadOnlyModel>(entity);
                var workflow = await _dbContext.WorkflowList.FindAsync(entity.WorkflowCode);
                resEntity.CreateUserName =
                    (await _dbContext.DncUser.FindAsync(entity.CreateUser))?.DisplayName;
                resEntity.WorkflowName = workflow?.Title;
                if (workflow != null)
                    resEntity.DateSpan =
                        $"{workflow.StartDate:yyyy-MM-dd HH:mm} 至 {workflow.EndDate:yyyy-MM-dd HH:mm}";
                var receivers = await _dbContext.WorkflowReceiver.Where(w => w.WorkflowCode == entity.WorkflowCode)
                    .ToListAsync();
                var notes = new List<Note>();
                foreach (var receiver in receivers)
                {
                    var note = new Note();
                    var step = await _dbContext.WorkflowStep
                        .FindAsync(receiver.StepCode);
                    note.NodeName = step?.Title;
                    var dic = await _dictionaryService.GetSYSDictionaryAsync("workflow_receiver_status",
                        receiver.Status);
                    note.StatusName = dic?.Name;
                    note.Status = receiver.Status;
                    var user = await _dbContext.DncUser
                        .FindAsync(receiver.User);
                    note.Department = user.Department?.Name;
                    note.Position = user.Position?.Name;
                    note.UserName = user?.DisplayName;
                    note.Opinion = receiver.Note;
                    note.NodeDate = receiver.CheckDate ==
                                    DateTime.MinValue
                        ? ""
                        : receiver.CheckDate.ToString("yyyy-MM-dd");
                    notes.Add(note);
                }

                var dic1 = await _dictionaryService.GetSYSDictionaryAsync("workflow_receiver_status", resEntity.Status);
                resEntity.StatusName = dic1?.Name;
                resEntity.Notes = notes;
                //审批步骤
                var currStep = await _dbContext.WorkflowStep.FindAsync(entity.StepCode);
                resEntity.Steps = await _dbContext.WorkflowStep.Where(s => s.TemplateCode == currStep.TemplateCode)
                    .Select(s => s.Title).ToListAsync();
                var unCheck =
                    await _dbContext.WorkflowReceiver.FirstOrDefaultAsync(f =>
                        f.WorkflowCode == entity.WorkflowCode && !f.IsCheck);
                var cur = await _dbContext.WorkflowStep.FindAsync(unCheck?.StepCode);
                if (cur != null)
                {
                    resEntity.CurrentStep = int.Parse(cur.SortID) + 1;
                }


                response.SetData(resEntity);
                return Ok(response);
            }
        }

        #region 私有方法

        /// <summary>
        /// 设置过期
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateStatus(int[] ids)
        {
            await using (_dbContext)
            {
                // var parameters = 
                //     ids.Select((id, index) => 
                //         new SqlParameter($"@p{index}", id)).ToList();
                //
                // var parameterNames = 
                //     string.Join(", ", parameters.
                //         Select(p => p.ParameterName));

                var formatIds = ids.Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE WorkflowReceiver SET Status='-1'  WHERE ID IN ({formatIds})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        #endregion
    }
}