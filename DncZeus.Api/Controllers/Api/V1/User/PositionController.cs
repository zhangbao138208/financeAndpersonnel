using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Entities.Enums;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.User.Position;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.User.Department;
using DncZeus.Api.ViewModels.User.Position;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Controllers.Api.V1.User
{
    [Route("api/v1/User/[controller]/[action]")]
    [ApiController]
    [CustomAuthorize]
    public class PositionController:ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;

        public PositionController(DncZeusDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult List(PositionRequestPaload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            using (_dbContext)
            {
                var query = _dbContext.UserPosition.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.Name.Contains(payload.Kw.Trim()) || x.Code.Contains(payload.Kw.Trim()));
                }
                if (payload.IsDeleted > CommonEnum.IsDeleted.All)
                {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (payload.Status > CommonEnum.Status.All)
                {
                    query = query.Where(x => x.Status == payload.Status);
                }
                var list = query.Paged(payload.CurrentPage, payload.PageSize).OrderBy(r => r.LevelID).ToList();
                var totalCount = query.Count();
                var data = list.Select(_mapper.Map<UserPosition, PositionJsonModel>);

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建职位
        /// </summary>
        /// <param name="model">职位视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public IActionResult Create(PositionCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Name.Trim().Length <= 0)
            {
                response.SetFailed("请输入职位名称");
                return Ok(response);
            }
            using (_dbContext)
            {
                if (_dbContext.DncRole.Count(x => x.Name == model.Name) > 0)
                {
                    response.SetFailed("职位已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<PositionCreateViewModel, UserPosition>(model);

                entity.CreatedOn = DateTime.Now;
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                entity.CreatedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;
               
                _dbContext.UserPosition.Add(entity);
                _dbContext.SaveChanges();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑职位
        /// </summary>
        /// <param name="code">职位惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public IActionResult Edit(string code)
        {
            using (_dbContext)
            {
                var entity = _dbContext.UserPosition.FirstOrDefault(x => x.Code == code);
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<UserPosition, PositionCreateViewModel>(entity);
                response.SetData(resEntity);
                return Ok(response);
            }
        }


        /// <summary>
        /// 保存编辑后的职位信息
        /// </summary>
        /// <param name="model">角色视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public IActionResult Edit(PositionCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }
            using (_dbContext)
            {
                if (_dbContext.UserPosition.Count(x => x.Name == model.Name && x.Code != model.Code) > 0)
                {
                    response.SetFailed("职位已存在");
                    return Ok(response);
                }

                var entity = _dbContext.UserPosition.Find(model.Code);
                entity.SortID = model.SortID;
                entity.Name = model.Name;
                entity.Status = model.Status;

                entity.ModifiedOn = DateTime.Now;
                entity.ModifiedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;
               
                _dbContext.Entry(entity).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除职位
        /// </summary>
        /// <param name="ids">职位code,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet("{ids}")]
        [ProducesResponseType(200)]
        public IActionResult Delete(string ids)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }
            response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
            return Ok(response);
        }

        /// <summary>
        /// 恢复职位
        /// </summary>
        /// <param name="ids">职位ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet("{ids}")]
        [ProducesResponseType(200)]
        public IActionResult Recover(string ids)
        {
            var response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
            return Ok(response);
        }
        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">职位ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult Batch(string command, string ids)
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
                    response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
                    break;
                case "recover":
                    response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
                    break;
                case "forbidden":
                    if (ConfigurationManager.AppSettings.IsTrialVersion)
                    {
                        response.SetIsTrial();
                        return Ok(response);
                    }
                    response = UpdateStatus(UserStatus.Forbidden, ids);
                    break;
                case "normal":
                    response = UpdateStatus(UserStatus.Normal, ids);
                    break;
                default:
                    break;
            }
            return Ok(response);
        }
        /// <summary>
        /// 查询所有职位列表(只包含主要的字段信息:name,code)
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/v1/user/position/find_simple_list")]
        public IActionResult FindSimpleList()
        {
            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext)
            {
                var roles = _dbContext.UserPosition.
                    Where(x => x.IsDeleted == CommonEnum.IsDeleted.No && x.Status == CommonEnum.Status.Normal).
                    OrderBy(r=>r.LevelID).Select(x => new { x.Name, x.Code }).ToList();
                response.SetData(roles);
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
        private ResponseModel UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids)
        {
            using (_dbContext)
            {
                var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = string.Format("UPDATE UserPosition SET IsDeleted=@IsDeleted WHERE Code IN ({0})", parameterNames);
                parameters.Add(new SqlParameter("@IsDeleted", (int)isDeleted));
                _dbContext.Database.ExecuteSqlCommand(sql, parameters);
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
        private ResponseModel UpdateStatus(UserStatus status, string ids)
        {
            using (_dbContext)
            {
                var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = string.Format("UPDATE UserPosition SET Status=@Status WHERE Code IN ({0})", parameterNames);
                parameters.Add(new SqlParameter("@Status", (int)status));
                _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion
    }
}

