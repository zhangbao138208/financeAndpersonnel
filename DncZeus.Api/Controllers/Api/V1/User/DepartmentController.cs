
using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Entities.Enums;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.User.Department;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.User.Department;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DncZeus.Api.Controllers.Api.V1.User
{
    [Route("api/v1/User/[controller]/[action]")]
    [ApiController]
    [CustomAuthorize]
    public class DepartmentController:ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;

        public DepartmentController(DncZeusDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult List(DepartmentRequestPaload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            using (_dbContext)
            {
                var query = _dbContext.UserDepartment.AsQueryable();
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
                var list = query.Paged(payload.CurrentPage, payload.PageSize).OrderBy(r=>r.SortID).ToList();
                var totalCount = query.Count();
                var data = list.Select(_mapper.Map<UserDepartment, DepartmentJsonModel>);

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="model">部门视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public IActionResult Create(DepartmentCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.Name.Trim().Length <= 0)
            {
                response.SetFailed("请输入部门名称");
                return Ok(response);
            }
            using (_dbContext)
            {
                if (_dbContext.DncRole.Count(x => x.Name == model.Name) > 0)
                {
                    response.SetFailed("角色已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<DepartmentCreateViewModel, UserDepartment>(model);
                
                entity.CreatedOn = DateTime.Now;
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                entity.CreatedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;
                foreach (var day in model.RestDays)
                {
                    switch (int.Parse(day))
                    {
                        case 1:
                            entity.Monday = null;
                            break;
                        case 2:
                            entity.Tuesday = null;
                            break;
                        case 3:
                            entity.Wednesday = null;
                            break;
                        case 4:
                            entity.Thursday = null;
                            break;
                        case 5:
                            entity.Friday = null;
                            break;
                        case 6:
                            entity.Saturday = null;
                            break;
                        case 7:
                            entity.Sunday = null;
                            break;
                        default:
                            break;
                    }
                }
                _dbContext.UserDepartment.Add(entity);
                _dbContext.SaveChanges();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑部门
        /// </summary>
        /// <param name="code">部门惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public IActionResult Edit(string code)
        {
            using (_dbContext)
            {
                var entity = _dbContext.UserDepartment.FirstOrDefault(x => x.Code == code);
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<UserDepartment, DepartmentCreateViewModel>(entity);
                List<string> restDays=new List<string>();

                if (string.IsNullOrEmpty(entity.Monday))
                {
                    restDays.Add("1");
                }
                else
                {
                    resEntity.WorkTime = entity.Monday;
                }

                if (string.IsNullOrEmpty(entity.Tuesday))
                {
                    restDays.Add("2");
                }
                else
                {
                    resEntity.WorkTime = entity.Tuesday;
                }

                if (string.IsNullOrEmpty(entity.Wednesday))
                {
                    restDays.Add("3");
                }
                else
                {
                    resEntity.WorkTime = entity.Wednesday;
                }

                if (string.IsNullOrEmpty(entity.Thursday))
                {
                    restDays.Add("4");
                }
                else
                {
                    resEntity.WorkTime = entity.Thursday;
                }

                if (string.IsNullOrEmpty(entity.Friday))
                {
                    restDays.Add("5");
                }
                else
                {
                    resEntity.WorkTime = entity.Friday;
                }
                if (string.IsNullOrEmpty(entity.Saturday))
                {
                    restDays.Add("6");
                }
                else
                {
                    resEntity.WorkTime = entity.Saturday;
                }

                if (string.IsNullOrEmpty(entity.Sunday))
                {
                    restDays.Add("7");
                }
                else
                {
                    resEntity.WorkTime = entity.Sunday;
                }
                resEntity.RestDays = restDays.ToArray();
                response.SetData(resEntity);
                return Ok(response);
            }
        }


        /// <summary>
        /// 保存编辑后的部门信息
        /// </summary>
        /// <param name="model">角色视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public IActionResult Edit(DepartmentCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }
            using (_dbContext)
            {
                if (_dbContext.UserDepartment.Count(x => x.Name == model.Name && x.Code != model.Code) > 0)
                {
                    response.SetFailed("部门已存在");
                    return Ok(response);
                }

                var entity = _dbContext.UserDepartment.Find(model.Code);
                entity.Monday = model.WorkTime;
                entity.Wednesday = model.WorkTime;
                entity.Thursday = model.WorkTime;
                entity.Tuesday = model.WorkTime;
                entity.Friday = model.WorkTime;
                entity.Saturday = model.WorkTime;
                entity.Sunday = model.WorkTime;
                entity.SortID = model.SortID;
                entity.Phone1 = model.Phone1;
                entity.Email = model.Email;
                entity.Name = model.Name;
                entity.Status = model.Status;

                entity.ModifiedOn = DateTime.Now;
                entity.ModifiedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;
                foreach (var day in model.RestDays)
                {
                    switch (int.Parse(day))
                    {
                        case 1:
                            entity.Monday = null;
                            break;
                        case 2:
                            entity.Tuesday = null;
                            break;
                        case 3:
                            entity.Wednesday = null;
                            break;
                        case 4:
                            entity.Thursday = null;
                            break;
                        case 5:
                            entity.Friday = null;
                            break;
                        case 6:
                            entity.Saturday = null;
                            break;
                        case 7:
                            entity.Sunday = null;
                            break;
                        default:
                            break;
                    }
                }
                _dbContext.Entry(entity).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="ids">部门code,多个以逗号分隔</param>
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
        /// 恢复部门
        /// </summary>
        /// <param name="ids">部门ID,多个以逗号分隔</param>
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
        /// <param name="ids">部门ID,多个以逗号分隔</param>
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
        /// 查询所有部门列表(只包含主要的字段信息:name,code)
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/v1/user/department/find_simple_list")]
        public IActionResult FindSimpleList()
        {
            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext)
            {
                var roles = _dbContext.UserDepartment.
                    Where(x => x.IsDeleted == CommonEnum.IsDeleted.No && x.Status == CommonEnum.Status.Normal).
                    OrderBy(r => r.LevelID).Select(x => new { x.Name, x.Code }).ToList();
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
                var sql = string.Format("UPDATE UserDepartment SET IsDeleted=@IsDeleted WHERE Code IN ({0})", parameterNames);
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
                var sql = string.Format("UPDATE UserDepartment SET Status=@Status WHERE Code IN ({0})", parameterNames);
                parameters.Add(new SqlParameter("@Status", (int)status));
                _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion
    }
}
