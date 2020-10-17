using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Extensions;
using DncZeus.Api.Models.Response;
using DncZeus.Api.Services;
using DncZeus.Api.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Extensions.DataAccess;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NLog;
using NPOI.SS.Formula.Functions;

namespace DncZeus.Api.Controllers.Api.V1
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly DictionaryService _dictionaryService;
        private readonly ILogger<HomeController> _logger;


        public HomeController(DncZeusDbContext dbContext, IMapper mapper, 
            DictionaryService dictionaryService,ILogger<HomeController> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _dictionaryService = dictionaryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseModel>> Init()
        {
            var colors = "0123456789ABCDEF".ToArray();
            var response = ResponseModelFactory.CreateInstance;
            HomeJsonModel homeJsonModel = new HomeJsonModel
            {
                CountCards = new List<CountCard>()
            };
            await using (_dbContext)
            {
                var dics = _dictionaryService.GetSYSSeting("dashboard_countCards");
                var random = new Random();
                //我的工作
                var color1 = "#";
                for (var j = 0; j < 6; j++)
                {
                    color1 += colors[random.Next(0, 15)];
                }
                var countCard1 = new CountCard
                {
                    Title = "我的工作",
                    Color = color1,
                    Count = AuthContextService.CurrentUser.UserType == UserType.SuperAdministrator
                        ?await _dbContext.WorkflowList.CountAsync():
                        await _dbContext.WorkflowList.
                            CountAsync(x=>x.User==AuthContextService.CurrentUser.Guid),
                    Route = (await _dbContext.DncMenu.FirstOrDefaultAsync(m => m.Name == "我的工作" && m.Level != 0))
                        ?.Alias,
                    Icon = (await _dbContext.DncMenu.FirstOrDefaultAsync(m => m.Name == "我的工作" && m.Level != 0))?.Icon
                };
                homeJsonModel.CountCards.Add(countCard1);
                
                //我的审批
                var color2 = "#";
                for (var j = 0; j < 6; j++)
                {
                    color1 += colors[random.Next(0, 15)];
                }
                var countCard2 = new CountCard
                {
                    Title = "我的审批",
                    Color = color2,
                    Count = AuthContextService.CurrentUser.UserType == UserType.SuperAdministrator
                        ?await _dbContext.WorkflowReceiver.CountAsync():
                        await _dbContext.WorkflowReceiver.
                            CountAsync(x=>x.User==AuthContextService.CurrentUser.Guid),
                    Route = (await _dbContext.DncMenu.FirstOrDefaultAsync(m => m.Name == "我的审批" && m.Level != 0))
                        ?.Alias,
                    Icon = (await _dbContext.DncMenu.FirstOrDefaultAsync(m => m.Name == "我的审批" && m.Level != 0))?.Icon
                };
                homeJsonModel.CountCards.Add(countCard2);
                
                foreach (var dic in dics.Value.ToString().Split("|"))
                {
                    var color = "#";
                    for (var j = 0; j < 6; j++)
                    {
                        color += colors[random.Next(0, 15)];
                    }
                    var countCard = new CountCard
                    {
                        Title = dic,
                        Color = color,
                        Count = await GetCount(dic),
                        Route = (await _dbContext.DncMenu.FirstOrDefaultAsync(m => m.Name == dic && m.Level != 0))
                            ?.Alias,
                        Icon = (await _dbContext.DncMenu.FirstOrDefaultAsync(m => m.Name == dic && m.Level != 0))?.Icon
                    };
                    homeJsonModel.CountCards.Add(countCard);
                }

                response.SetData(homeJsonModel);
                return Ok(response);
            }

        }

        #region 私有方法

        /// <summary>
        /// 动态获取
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Obsolete]
        private async Task<int> GetCount(string name)
        {

           var dic =  _dictionaryService.GetSYSSeting(name);
           
            var ret = await _dbContext.Set(dic.Value).CountAsync();

           return ret;

           #endregion
        }
    }
}
