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
    [CustomAuthorize]
    public class ReceiverController:ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly DictionaryService _dictionaryService;

        public ReceiverController(DncZeusDbContext dbContext, IMapper mapper,
            DictionaryService dictionaryService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _dictionaryService = dictionaryService;
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
            using (_dbContext)
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
                             from User in t3.DefaultIfEmpty()

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
                                 Id=ls.Id,
                                 CheckDate=ls.CheckDate,
                                 CreateDate=ls.CreateDate,
                                 CreateUser=ls.CreateUser,
                                 Note=ls.Note,
                                 StepCode=ls.StepCode,
                                 WorkflowCode=ls.WorkflowCode,
                                 IsCheck=ls.IsCheck,

                                 Status = ls.Status,
                                 TemplateCode = ls.TemplateCode,
                                 ListType = ls.Type,
                                 User = ls.User,
                                 TemplateName = tl.Name,
                                 UserName = User.DisplayName,
                                 WorkflowName=w.Title,
                                 StepName=step1.Title,
                                 
                             });
                query = query.Where(q => q.User == AuthContextService.CurrentUser.Guid);
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.UserName.Contains(payload.Kw.Trim()) ||
                   x.Note.Contains(payload.Kw.Trim()));
                }
                if (!string.IsNullOrEmpty(payload.Status)&&payload.Status!="all")
                {
                    query = query.Where(x => x.Status==payload.Status.Trim());
                }
                var list = await query.Paged(payload.CurrentPage, payload.PageSize).ToListAsync();
                foreach (var r in list)
                {
                    var dic = await _dictionaryService.GetSYSDictionaryAsync("workflow_list_type", r.ListType);
                    r.ListTypeName = dic?.Name;
                    var dic1 = await _dictionaryService.GetSYSDictionaryAsync("workflow_receiver_status", r.Status);
                    r.StatusName = dic1?.Name;
                    //只要会签中有一个不同意都不能编辑 或签只能选一个人所以不会出现这种情况
                    r.CanEdit = !(await _dbContext.WorkflowReceiver.
                        AnyAsync(a=>a.Status=="2"&&a.WorkflowCode==r.WorkflowCode));
                }
                var totalCount = query.Count();
                var data = list;

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
            using (_dbContext)
            {

                var entity = await _dbContext.WorkflowReceiver.FindAsync(int.Parse(code));
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<WorkflowReceiver, ReceiverEditViewModel>(entity);
                var workflow = await _dbContext.WorkflowList.FindAsync(entity.WorkflowCode);
                resEntity.WorkflowName = workflow?.Title;
                resEntity.ListType = workflow?.Type;
                resEntity.EndDate = workflow?.EndDate.ToString("yyyy-MM-dd HH:MM:ss");

                //var currStep1 = _dbContext.WorkflowStep.Find(entity.StepCode);
                //var currStep2 = await 
                //    _dbContext.WorkflowStep.FindAsync(entity.StepCode).ConfigureAwait(true);
                //var currStep3 =_dbContext.WorkflowStep.FirstOrDefaultAsync(f=>f.Code== entity.StepCode);
                //查找下一步
                var currStep = await _dbContext.WorkflowStep.FindAsync(entity.StepCode);
                var nextStep = await  _dbContext.WorkflowStep.
                    FirstOrDefaultAsync(n => n.SortID == (int.Parse(currStep.SortID) + 1).ToString());

                resEntity.NextStepCode = nextStep?.Code;
                resEntity.NextStepName = nextStep?.Title;
                //下拉列表数据
                if (nextStep!=null)
                {
                    var users =await  _dbContext.DncUser.Where(u => nextStep.UserList.Contains(u.Guid.ToString())).
                        Select(s => new NextUser
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
            using (_dbContext)
            {
                if (model.IsCheck)
                {
                    response.SetFailed("改审批已完成");
                    return Ok(response);
                }
                if (!model.IsCheck&&model.Status==-1)
                {
                    response.SetFailed("改审批已过期");
                    return Ok(response);
                }
                if (await _dbContext.WorkflowReceiver.
                        AnyAsync(a => a.Status == "2" && a.WorkflowCode == model.WorkflowCode))
                {
                    response.SetFailed("该审批已拒绝");
                    return Ok(response);
                }


                // var entity = _mapper.Map<ReceiverCreateViewModel, WorkflowReceiver>(model);
                var entity = await _dbContext.WorkflowReceiver.FindAsync(model.Id);

                //进行下一步审批
                if (!string.IsNullOrEmpty(model.NextStepCode) 
                    && model.Approver != null&&model.Status==1)
                {
                    
                    var step = await _dbContext.WorkflowStep.FindAsync(model.NextStepCode);
                    var nextStepCode = "";
                    if (step != null)
                    {
                        var steps = await _dbContext.WorkflowStep.
                            Where(s => s.TemplateCode == step.TemplateCode).ToListAsync();
                        if (steps.Count > int.Parse(step.SortID))
                        {
                            nextStepCode = steps[int.Parse(step.SortID)]?.Code;
                        }
                    }
                    List<WorkflowReceiver> receivers = new List<WorkflowReceiver>();
                    foreach (var u in model.Approver)
                    {
                        //发送审批到下一个人
                        var receiver = WorkflowReceiverFactory.CreateInstance;
                        receiver.Type = entity.Type;
                        receiver.User = u;
                        receiver.Description = entity.Description;
                        receiver.WorkflowCode = entity.WorkflowCode;
                        receiver.CreateUser = AuthContextService.CurrentUser.Guid;
                        receiver.StepCode = nextStepCode;
                        receiver.TemplateCode = entity.TemplateCode;
                        receivers.Add(receiver);
                    }

                    _dbContext.WorkflowReceiver.AddRange(receivers);
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
            using (_dbContext)
            {
                var entity = await _dbContext.WorkflowReceiver.FindAsync(int.Parse(code));
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<WorkflowReceiver, ReceiverReadOnlyModel>(entity);
                var workflow = await _dbContext.WorkflowList.FindAsync(entity.WorkflowCode);
                resEntity.CreateUserName = 
                    (await _dbContext.DncUser.FindAsync(entity.CreateUser))?.DisplayName;
                resEntity.WorkflowName = workflow?.Title;
                resEntity.DateSpan = $"{workflow.StartDate.ToString("yyyy-MM-dd HH:mm")} 至 {workflow.EndDate.ToString("yyyy-MM-dd HH:mm")}";
                var receivers = await _dbContext.WorkflowReceiver.
                    Where(w => w.WorkflowCode == entity.WorkflowCode).ToListAsync();
                var notes = new List<Note>();
                foreach (var receiver in receivers)
                {
                    var note = new Note();
                    var step = await _dbContext.WorkflowStep.FindAsync(receiver.StepCode);
                    note.NodeName = step?.Title;
                    var dic = await _dictionaryService.GetSYSDictionaryAsync("workflow_receiver_status", receiver.Status);
                    note.StatusName = dic?.Name;
                    note.Status = receiver.Status;
                    var user = await _dbContext.DncUser.FindAsync(receiver.User);
                    note.UserName = user?.DisplayName;
                    note.Opinion = receiver.Note;
                    note.NodeDate = receiver.CheckDate==
                        DateTime.MinValue?"":receiver.CheckDate.ToString("yyyy-MM-dd");
                    notes.Add(note);
                }

                var dic1 = await _dictionaryService.GetSYSDictionaryAsync("workflow_receiver_status", resEntity.Status);
                resEntity.StatusName = dic1?.Name;
                resEntity.Notes = notes;
                //审批步骤
                var currStep = await _dbContext.WorkflowStep.FindAsync(entity.StepCode);
                resEntity.Steps = await  _dbContext.WorkflowStep.Where(s => s.TemplateCode == currStep.TemplateCode)
                    .Select(s=>s.Title).ToListAsync();
                var unCheck = await _dbContext.WorkflowReceiver.
                    FirstOrDefaultAsync(f=>f.WorkflowCode==entity.WorkflowCode&&!f.IsCheck);
                var cur = await _dbContext.WorkflowStep.FindAsync(unCheck?.StepCode);
                resEntity.currentStep = int.Parse(cur.SortID) + 1;

                response.SetData(resEntity);
                return Ok(response);
            }
        }
    }
}
