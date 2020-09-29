using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.System.Dictionary;
using DncZeus.Api.Services;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.System.Dictionary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Controllers.Api.V1.System
{
    [Route("api/v1/system/[controller]/[action]")]
    [ApiController]
    [CustomAuthorize]
    public class DictionaryController:ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly DictionaryService _dictionaryService;

        public DictionaryController(DncZeusDbContext dbContext, IMapper mapper, 
            DictionaryService dictionaryService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _dictionaryService = dictionaryService;
        }
        /// <summary>
        /// 字典列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ResponseResultModel<IEnumerable<DictionaryJsonModel>>> List(DictionaryRequestPaload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            using (_dbContext)
            {
                var query = (from dic in _dbContext.SystemDictionary
                             join type in _dbContext.SystemDicType on dic.TypeCode equals type.Code
                             into t1
                             from type in t1.DefaultIfEmpty()
                             select new DictionaryJsonModel
                             {
                                 Code=dic.Code,
                                 Name=dic.Name,
                                 Fixed=dic.Fixed,
                                 TypeCode=dic.TypeCode,
                                 TypeName=type.Name,
                                 Value=dic.Value,
                             });
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.Name.Contains(payload.Kw.Trim()) ||
                   x.Code.Contains(payload.Kw.Trim()) || x.Value.Contains(payload.Kw.Trim()));
                }
                var list = query.Paged(payload.CurrentPage, payload.PageSize).OrderBy(r=>r.TypeCode).ToList();
                var totalCount = query.Count();
                var data = list;

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建字典
        /// </summary>
        /// <param name="model">字典视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public ActionResult<ResponseModel> Create(DictionaryCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Name.Trim().Length <= 0)
            {
                response.SetFailed("请输入字典名称");
                return Ok(response);
            }
            using (_dbContext)
            {
                if (_dbContext.DncRole.Count(x => x.Name == model.Name) > 0)
                {
                    response.SetFailed("字典已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<DictionaryCreateViewModel, SystemDictionary>(model);
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                _dbContext.SystemDictionary.Add(entity);
                _dbContext.SaveChanges();
                //更新缓存
                _dictionaryService.ClearDictionaryCache();
                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑字典
        /// </summary>
        /// <param name="code">字典惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public ActionResult<ResponseModel<DictionaryCreateViewModel>> Edit(string code)
        {
            using (_dbContext)
            {
                var entity = _dbContext.SystemDictionary.FirstOrDefault(x => x.Code == code);
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<SystemDictionary, DictionaryCreateViewModel>(entity);
                response.SetData(resEntity);
                return Ok(response);
            }
        }


        /// <summary>
        /// 保存编辑后的字典信息
        /// </summary>
        /// <param name="model">字典视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public ActionResult<ResponseModel> Edit(DictionaryCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }
            using (_dbContext)
            {
                if (_dbContext.SystemDictionary.Count(x => x.Name == model.Name && x.Code != model.Code) > 0)
                {
                    response.SetFailed("字典已存在");
                    return Ok(response);
                }

                var entity = _mapper.Map<DictionaryCreateViewModel, SystemDictionary>(model);
                _dbContext.Entry(entity).State = EntityState.Modified;
                _dbContext.SaveChanges();
                //更新缓存
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
        public ActionResult<ResponseModel> Delete(string ids)
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
            using (_dbContext)
            {
                _dbContext.SystemDictionary.RemoveRange(_dbContext.SystemDictionary
                    .Where(w=>ids.Contains(w.Code)).ToList()
                    );
                _dbContext.SaveChanges();
                //更新缓存
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
        public ActionResult<ResponseModel> Batch(string command, string ids)
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
                    response= UpdateIsDelete(ids);
                    //更新缓存
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
        /// 查询所有字典列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/v1/system/dictionary/find_simple_list/{code}")]
        public  ActionResult<ResponseResultModel<IEnumerable<DictionaryJsonModel>>> FindSimpleList(string code)
        {
            var response = ResponseModelFactory.CreateInstance;
            var list =  _dictionaryService.GetSYSDictionary(code);
            var data= list;
            response.SetData(data);
            return Ok(response);
        }
        #region 私有方法

        /// <summary>
        /// 删除财务账号
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">财务账号ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private ResponseModel UpdateIsDelete(string ids)
        {
            using (_dbContext)
            {
                var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = string.Format("DELETE SystemDictionary  WHERE Code IN ({0})", parameterNames);
                _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion

    }
}
