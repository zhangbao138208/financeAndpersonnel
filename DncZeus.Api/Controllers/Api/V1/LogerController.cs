using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.System.Loger;
using DncZeus.Api.ViewModels.System.Loger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;

namespace DncZeus.Api.Controllers.Api.V1
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    [ServiceFilter(typeof(CustomAuthorizeAttribute))]
    public class LogerController: ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;
        
        public LogerController(
            DncZeusDbContext dbContext,
            IMapper mapper )
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        /// <summary>
        /// 日志列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<SystemLogJsonModel>>>>
            List(LogerRequestPaload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            if (payload.Start != null)
            {
                var d = string.Join("", payload.Start.ToString().Split('/', '-', ':'));
            }
            await using (_dbContext)
            {
                var query = _dbContext.SystemLog.AsQueryable();
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.
                        Requesturl.Contains(payload.Kw.Trim()) ||
                                             x.Action.Contains(payload.Kw.Trim())||
                                             x.Levels.Contains(payload.Kw.Trim()));
                }
                

                if (payload.Start.HasValue && payload.End.HasValue)
                {

                    query = query.Where(x =>
                        // ReSharper disable once StringCompareIsCultureSpecific.1
                        string.Compare(x.Operatingtime, 
                            payload.Start.ToString().Replace('-','/')) >=
                        0 &&
                        // ReSharper disable once StringCompareIsCultureSpecific.1
                        string.Compare(x.Operatingtime, 
                            payload.End.ToString().Replace('-','/')) <= 0);
                }
                // var list = await query.Paged(payload.CurrentPage, payload.PageSize).ToListAsync();
                var list = await query.OrderByDescending(r =>r.Operatingtime)
                    .Paged(payload.CurrentPage, payload.PageSize)
                    .ToListAsync();
                var totalCount = await query.CountAsync();
                var data = list.Select(_mapper.Map<SystemLog, SystemLogJsonModel>);

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }
    }
}