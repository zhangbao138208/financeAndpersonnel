using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Entities.Enums;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.Workflow.Template;
using DncZeus.Api.Services;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.Workflow.Template;
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
    [Route("api/v1/workflow/[controller]/[action]")]
    [ApiController]
    // [ServiceFilter(typeof(CustomAuthorizeAttribute))]
    [ServiceFilter(typeof(CustomAuthorizeAttribute))]
    public class TemplateController:ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;

        public TemplateController(DncZeusDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        /// <summary>
        /// 模板列表
        /// </summary> 
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<TemplateJsonModel>>>> 
            List(TemplateRequestPayload payload)
        {
            await using (_dbContext)
            {
                var query = (from template in _dbContext.WorkflowTemplate
                             join copy in _dbContext.WorkflowTemplate on 
                             template.ParentCode equals copy.Code
                             into t1
                             from copy in t1.DefaultIfEmpty()
                            
                             select new TemplateJsonModel
                             {
                                 Code = template.Code,
                                 IsDeleted=template.IsDeleted,
                                 IsStepFree=template.IsStepFree,
                                 Description=template.Description,
                                 Name=template.Name,
                                 ParentCode=template.ParentCode,
                                 ParentName=copy.Name,
                                 Visible=template.Visible,
                                 
                                 Status = template.Status,
                                 CreatedOn = template.CreatedOn.ToString(),
                                 CreatedByUserGuid = template.CreatedByUserGuid,
                                 CreatedByUserName = template.CreatedByUserName,
                                 ModifiedOn = template.ModifiedOn.ToString(),
                                 ModifiedByUserGuid = template.ModifiedByUserGuid,
                                 ModifiedByUserName = template.ModifiedByUserName
                             });
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.Name.Contains(payload.Kw.Trim()) || x.Description.Contains(payload.Kw.Trim()));
                }
                if (payload.IsDeleted > CommonEnum.IsDeleted.All)
                {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (payload.Status > CommonEnum.Status.All)
                {
                    query = query.Where(x => x.Status == payload.Status);
                }
                if (payload.ParentCode!="all")
                {
                    query = query.Where(x => 
                        x.ParentCode == payload.ParentCode);
                }
                //if (payload.ParentGuid.HasValue)
                //{
                //    query = query.Where(x => x.ParentGuid == payload.ParentGuid);
                //}
                var list = await query.Paged(payload.CurrentPage, payload.PageSize).ToListAsync();
                var totalCount = query.Count();
                var data = list;
                var response = ResponseModelFactory.CreateResultInstance;
                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="model">模板视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Create(TemplateCreateViewModel model)
        {
            await using (_dbContext)
            {
                var entity = _mapper.Map<TemplateCreateViewModel, WorkflowTemplate>(model);
                entity.CreatedOn = DateTime.Now;
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                entity.CreatedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;
                entity.IsDeleted = CommonEnum.IsDeleted.No;
                await _dbContext.WorkflowTemplate.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                var response = ResponseModelFactory.CreateInstance;
                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑模板
        /// </summary>
        /// <param name="code">模板ID</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<TemplateEditRetModel>> Edit(string code)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.WorkflowTemplate.FindAsync(code);
                var response = ResponseModelFactory.CreateInstance;
                var model = _mapper.Map<WorkflowTemplate,TemplateEditViewModel>(entity);
                var tree = await LoadMenuTree(model.ParentCode);
                model.ParentName =  tree.FirstOrDefault
                    (t=>t.Code==model.ParentCode)?.Title;

                response.SetData(new { model, tree });
                return Ok(response);
            }
        }

        /// <summary>
        /// 保存编辑后的模板信息
        /// </summary>
        /// <param name="model">模板视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<ActionResult<TemplateEditRetModel>> Edit(TemplateEditViewModel model)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.WorkflowTemplate.FindAsync(model.Code);
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.ParentCode = model.ParentCode;
                entity.Visible = model.Visible;
                entity.IsStepFree = model.IsStepFree;
                
                entity.ModifiedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;
                entity.ModifiedOn = DateTime.Now;
                if (!ConfigurationManager.AppSettings.IsTrialVersion)
                {
                    entity.Status = model.Status;
                   
                }
                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                var response = ResponseModelFactory.CreateInstance;
                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 模板树
        /// </summary>
        /// <returns></returns>
        [HttpGet("{selected?}")]
        public async Task<ActionResult<TemplateTree>> Tree(string selected)
        {
            var response = ResponseModelFactory.CreateInstance;
            var tree = await LoadMenuTree(selected?.ToString());
            response.SetData(tree);
            return Ok(response);
        }

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="ids">模板ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpDelete("{ids}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(string ids)
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
        /// 恢复模板
        /// </summary>
        /// <param name="ids">模板ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpPost("{ids}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Recover(string ids)
        {
            var response = await UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
            return Ok(response);
        }

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">模板ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Batch(string command, string ids)
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
        [HttpGet("/api/v1/Workflow/template/find_simple_list")]
        public ActionResult<ResponseResultModel<IEnumerable<SimpleModel>>> FindSimpleList()
        {
            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext)
            {
                var roles = _dbContext.WorkflowTemplate.
                    Where(x => x.IsDeleted == CommonEnum.IsDeleted.No &&
                    x.Status == CommonEnum.Status.Normal).
                    Select(x => new { x.Name,x.Description, x.Code }).ToList();
                response.SetData(roles);
            }
            return Ok(response);
        }

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">模板ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids)
        {
            await using (_dbContext)
            {
                // var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                // var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                // var sql = $"UPDATE WorkflowTemplate SET IsDeleted=@IsDeleted WHERE Guid IN ({parameterNames})";
                // parameters.Add(new SqlParameter("@IsDeleted", (int)isDeleted));
                // await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE WorkflowTemplate SET IsDeleted={(int)isDeleted} WHERE Code IN ({formatIds})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="status">模板状态</param>
        /// <param name="ids">模板ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateStatus(UserStatus status, string ids)
        {
            await using (_dbContext)
            {
                // var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                // var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                // var sql = $"UPDATE WorkflowTemplate SET Status=@Status WHERE Guid IN ({parameterNames})";
                // parameters.Add(new SqlParameter("@Status", (int)status));
                // await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE WorkflowTemplate SET Status={(int)status} WHERE Code IN ({formatIds})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        private async Task<List<TemplateTree>> LoadMenuTree(string selectedCode = null)
        {
            var temp =(await  _dbContext.WorkflowTemplate.
                Where(x => x.IsDeleted == CommonEnum.IsDeleted.No && 
                           x.Status == CommonEnum.Status.Normal).ToListAsync()).
                Select(x => new TemplateTree
                {
                Code=x.Code,
                ParentCode = x.ParentCode,
                Title = x.Name
            }).ToList();
            var root = new TemplateTree
            {
                Title = "顶级模板",
                Code = null,
                ParentCode = null
            };
            temp.Insert(0, root);
            var tree = temp.BuildTree(selectedCode);
            return tree;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class MenuTreeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="templates"></param>
        /// <param name="selectedCode"></param>
        /// <returns></returns>
        public static List<TemplateTree> BuildTree(this List<TemplateTree> templates, string selectedCode = null)
        {
            var lookup = templates.ToLookup(x => x.ParentCode);
            Func<string, List<TemplateTree>> build = null;
            build = pid =>
            {
                return lookup[pid]
                 .Select(x => new TemplateTree()
                 {
                     Code = x.Code,
                     ParentCode = x.ParentCode,
                     Title = x.Title,
                     Expand = (string.IsNullOrEmpty(x.ParentCode)),
                     Selected = selectedCode == x.Code,
                     Children = build(x.Code??""),
                 })
                 .ToList();
            };
            var result = build(null);
            return result;
        }
    }
}

