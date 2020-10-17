using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.System.DicType;
using DncZeus.Api.Services;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.System.DicType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Controllers.Api.V1.System
{
    [Route("api/v1/system/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(CustomAuthorizeAttribute))]
    public class DictionaryTypeController : ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly DictionaryService _dictionaryService;

        public DictionaryTypeController(DncZeusDbContext dbContext, IMapper mapper,
            DictionaryService dictionaryService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _dictionaryService = dictionaryService;
        }
        /// <summary>
        /// 字典类型列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<DicTypeJsonModel>>>> 
            List(DicTypeRequestPaload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            await using (_dbContext)
            {
                var query = _dbContext.SystemDicType.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.Name.Contains(payload.Kw.Trim()) ||
                    x.Code.Contains(payload.Kw.Trim())||x.Value.Contains(payload.Kw.Trim()));
                }
                var list = await query.Paged(payload.CurrentPage, payload.PageSize).ToListAsync();
                var totalCount = query.Count();
                var data = list.Select(_mapper.Map<SystemDicType, DicTypeJsonModel>);

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建字典类型
        /// </summary>
        /// <param name="model">字典类型视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Create(DicTypeCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Name.Trim().Length <= 0)
            {
                response.SetFailed("请输入字典类型名称");
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (await _dbContext.DncRole.CountAsync(x => x.Name == model.Name) > 0)
                {
                    response.SetFailed("字典类型已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<DicTypeCreateViewModel, SystemDicType>(model);
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                await _dbContext.SystemDicType.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                //清理缓存
                _dictionaryService.ClearDictionaryCache();
                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑字典类型
        /// </summary>
        /// <param name="code">字典类型惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel<DicTypeCreateViewModel>>> Edit(string code)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.SystemDicType
                    .FirstOrDefaultAsync(x => x.Code == code);
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<SystemDicType, DicTypeCreateViewModel>(entity);
                response.SetData(resEntity);
                return Ok(response);
            }
        }


        /// <summary>
        /// 保存编辑后的字典类型信息
        /// </summary>
        /// <param name="model">字典类型视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Edit(DicTypeCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (_dbContext.SystemDicType.Count(x => x.Name == model.Name && x.Code != model.Code) > 0)
                {
                    response.SetFailed("字典已存在");
                    return Ok(response);
                }

                var entity = _mapper.Map<DicTypeCreateViewModel, SystemDicType>(model);
                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                //清理缓存
                _dictionaryService.ClearDictionaryCache();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除字典
        /// </summary>
        /// <param name="ids">字典code,多个以逗号分隔</param>
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

            await using (_dbContext)
            {
                _dbContext.SystemDicType.RemoveRange(_dbContext.SystemDicType
                    .Where(w => ids.Contains(w.Code)).ToList()
                    );
                await _dbContext.SaveChangesAsync();
                //清理缓存
                _dictionaryService.ClearDictionaryCache();
            }

            return Ok(response);
        }


        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">字典ID,多个以逗号分隔</param>
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
                    response =await UpdateIsDelete(ids);
                    //清理缓存
                    _dictionaryService.ClearDictionaryCache();
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
        /// <summary>
        /// 查询所有字典类型列表(只包含主要的字段信息:name,code)
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/v1/system/dictionaryType/find_simple_list")]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<DicTypeJsonModel>>>> FindSimpleList()
        {
            var response = ResponseModelFactory.CreateInstance;
            await using (_dbContext)
            {
                var list =await  _dbContext.SystemDicType.ToListAsync();
                var data = list.Select(_mapper.Map<SystemDicType, DicTypeJsonModel>);

                response.SetData(data);
            }
            return Ok(response);
        }
        #region 私有方法

        /// <summary>
        /// 删除财务账号
        /// </summary>
        /// <param name="ids">财务账号ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateIsDelete(string ids)

        {
            await using (_dbContext)
            {
                // var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                // var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                // var sql = string.Format("DELETE SystemDicType  WHERE Code IN ({0})", parameterNames);
                // _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                var formatIds = ids.Split(',').Aggregate("", (current, id) => current + $"'{id}',");
                formatIds = formatIds.Substring(0, formatIds.Length - 1);
                var sql = $"DELETE SystemDicType WHERE Code IN ({formatIds})";
                await _dbContext.Database.ExecuteSqlRawAsync(sql);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion
    }
}
