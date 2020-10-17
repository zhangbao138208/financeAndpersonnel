using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Entities.Enums;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.Finance.Account;
using DncZeus.Api.RequestPayload.Finance.FinanceInfo;
using DncZeus.Api.Services;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.Finance.Account;
using DncZeus.Api.ViewModels.Finance.FinanceInfo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Controllers.Api.V1.Finance
{
    [Route("api/v1/finance/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(CustomAuthorizeAttribute))]
    public class FinanceInfoController:ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly DictionaryService _dictionaryService;

        public FinanceInfoController(
            DncZeusDbContext dbContext,
            IMapper mapper, 
            DictionaryService dictionaryService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _dictionaryService = dictionaryService;
        }

        /// <summary>
        /// 财务管理列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<FinanceInfoJsonModel>>>> 
            List(FinanceInfoRequestPayload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            await using (_dbContext)
            {
                var query = (
                    from f in _dbContext.FinanceInfo
                    join account in _dbContext.FinanceAccount
                            on f.FinanceAccount equals account.Code
                            into t1
                    from account in t1.DefaultIfEmpty()
                    join department in _dbContext.UserDepartment 
                             on f.DepartmentCode equals department.Code
                             into t2
                             from department in t2.DefaultIfEmpty()
                             select new FinanceInfoJsonModel
                             {
                                 Title=f.Title,
                                 IsDeleted=f.IsDeleted,
                                 User=f.User,
                                 FinanceAccount=f.FinanceAccount,
                                 FinanceAccountName=account.Name,
                                 FilePath =f.FilePath,
                                 ImagePath=f.ImagePath,
                                 InfoStatus=f.InfoStatus,
                                 Amount=f.Amount,
                                 HandleDate=f.HandleDate,
                                 HandleName=f.HandleName,
                                 Description=f.Description,
                                 Note=f.Note,
                                 Type=f.Type,
                                 DepartmentCode = f.DepartmentCode,
                                 DepartmentName = department.Name,
                                 Code = f.Code,
                                 CreatedOn = f.CreatedOn.ToString(),
                                 CreatedByUserGuid = f.CreatedByUserGuid,
                                 CreatedByUserName = f.CreatedByUserName,
                                 ModifiedOn = f.ModifiedOn.ToString(),
                                 ModifiedByUserGuid = f.ModifiedByUserGuid,
                                 ModifiedByUserName = f.ModifiedByUserName
                             });
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.Title.Contains(payload.Kw.Trim()) || x.Code.Contains(payload.Kw.Trim()));
                }
                if (payload.IsDeleted > CommonEnum.IsDeleted.All)
                {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (!string.IsNullOrWhiteSpace(payload.Status))
                {
                    query = query.Where(x => x.InfoStatus == payload.Status);
                }
                var list = await query.Paged(payload.CurrentPage, payload.PageSize).ToListAsync();
                var totalCount = await query.CountAsync();
                //var data = list.Select(_mapper.Map<FinanceInfo, FinanceInfoJsonModel>).ToList();
                var data = list;
                data.ForEach(r => {
                    var dic = _dictionaryService.GetSYSDictionary("finance_manager_type", r.Type);
                    r.TypeName = dic?.Name;
                    var dic1 = _dictionaryService.GetSYSDictionary("finance_manager_status", r.InfoStatus);
                    r.InfoStatusName = dic1?.Name;
                    // r.InfoStatusName = dic?.Name;
                });


                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建财务管理
        /// </summary>
        /// <param name="model">财务管理视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Create(FinanceInfoCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Title.Trim().Length <= 0)
            {
                response.SetFailed("请输入财务管理名称");
                return Ok(response);
            }

            await using (_dbContext)
            {
                var entity = _mapper.Map<FinanceInfoCreateViewModel, FinanceInfo>(model);

                entity.CreatedOn = DateTime.Now;
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                entity.CreatedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;

                await _dbContext.FinanceInfo.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑财务管理
        /// </summary>
        /// <param name="code">财务管理惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel<FinanceInfoCreateViewModel>>> Edit(string code)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.FinanceInfo.FirstOrDefaultAsync(x => x.Code == code);
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<FinanceInfo, FinanceInfoCreateViewModel>(entity);
                response.SetData(resEntity);
                return Ok(response);
            }
        }


        /// <summary>
        /// 保存编辑后的财务管理信息
        /// </summary>
        /// <param name="model">财务管理视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Edit(FinanceInfoCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }

            await using (_dbContext)
            {
                //if (_dbContext.FinanceInfo.Count(x => x.Name == model.Name && x.Code != model.Code) > 0)
                //{
                //    response.SetFailed("财务管理已存在");
                //    return Ok(response);
                //}

                var entity = _mapper.Map<FinanceInfoCreateViewModel, FinanceInfo>(model);

                entity.ModifiedOn = DateTime.Now;
                entity.ModifiedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;

                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除财务管理
        /// </summary>
        /// <param name="ids">财务管理code,多个以逗号分隔</param>
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
        /// 恢复财务管理
        /// </summary>
        /// <param name="ids">财务管理ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpPost("{ids}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Recover(string ids)
        {
            var response =await UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
            return Ok(response);
        }
        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">财务管理ID,多个以逗号分隔</param>
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
                    response =await UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
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
        /// 删除财务管理
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">财务管理ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids)
        {
            await using (_dbContext)
            {
                // var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                // var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                // var sql = string.Format("UPDATE FinanceInfo SET IsDeleted=@IsDeleted WHERE Code IN ({0})", parameterNames);
                // parameters.Add(new SqlParameter("@IsDeleted", (int)isDeleted));
                // _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE FinanceInfo SET IsDeleted={(int)isDeleted} WHERE Code IN ({formatIds})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }

        /// <summary>
        /// 删除财务管理
        /// </summary>
        /// <param name="status">财务管理状态</param>
        /// <param name="ids">财务管理ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateStatus(UserStatus status, string ids)
        {
            await using (_dbContext)
            {
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"UPDATE FinanceInfo SET Status={(int)status} WHERE Code IN ({formatIds})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion
    }
}
