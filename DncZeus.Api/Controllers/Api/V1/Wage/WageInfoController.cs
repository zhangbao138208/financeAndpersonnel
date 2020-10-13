using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.Entities.Enums;
using DncZeus.Api.Extensions;
using DncZeus.Api.Extensions.AuthContext;
using DncZeus.Api.Extensions.CustomException;
using DncZeus.Api.Models.Response;
using DncZeus.Api.RequestPayload.Wage;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.Wage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Controllers.Api.V1.Wage
{
    [Route("api/v1/Wage/[controller]/[action]")]
    [ApiController]
    [CustomAuthorize]
    public class WageInfoController : ControllerBase
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;
        public WageInfoController(DncZeusDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        /// <summary>
        /// 工资
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<WageJsonModel>>>>
            List(WageRequestPayload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            await using (_dbContext)
            {
                var query = (from wageInfo in _dbContext.WageInfo
                             join position in _dbContext.UserPosition on wageInfo.PositionCode equals position.Code
                             into t1
                             from position in t1.DefaultIfEmpty()
                             join department in _dbContext.UserDepartment on wageInfo.DepartmentCode equals department.Code
                             into t2
                             from department in t2.DefaultIfEmpty()
                             select new WageJsonModel
                             {
                                 Code = wageInfo.Code,
                                 RealName = wageInfo.RealName,
                                 PositionCode = wageInfo.PositionCode,
                                 PositionName = position.Name,
                                 DepartmentCode = wageInfo.DepartmentCode,
                                 DepartmentName = department.Name,
                                 StartDate = wageInfo.StartDate,
                                 EndDate = wageInfo.EndDate,
                                 BaseWage = wageInfo.BaseWage,
                                 WorkDays = wageInfo.WorkDays,
                                 OTDays = wageInfo.OTDays,
                                 OTWage = wageInfo.OTWage,
                                 PerformanceWage = wageInfo.PerformanceWage,
                                 ReissueWage = wageInfo.ReissueWage,
                                 Commission = wageInfo.Commission,
                                 Bonus = wageInfo.Bonus,
                                 SocialSecurity = wageInfo.SocialSecurity,
                                 Additions = wageInfo.Additions,
                                 Subsidy = wageInfo.Subsidy,
                                 AccumulationFund = wageInfo.AccumulationFund,
                                 TotalWage = wageInfo.TotalWage,
                                 IncomeTax = wageInfo.IncomeTax,
                                 Deductions = wageInfo.Deductions,
                                 IsDeleted = wageInfo.IsDeleted,
                                 Status = wageInfo.Status,
                                 CreatedOn = wageInfo.CreatedOn.ToString(),
                                 CreatedByUserGuid = wageInfo.CreatedByUserGuid,
                                 CreatedByUserName = wageInfo.CreatedByUserName,
                                 ModifiedOn = wageInfo.ModifiedOn.ToString(),
                                 ModifiedByUserGuid = wageInfo.ModifiedByUserGuid,
                                 ModifiedByUserName = wageInfo.ModifiedByUserName
                             });
                if (payload.Start.HasValue && payload.End.HasValue)
                {
                    query = query.Where(w => w.StartDate >= payload.Start && w.EndDate <= payload.End);
                }
                if (!string.IsNullOrWhiteSpace(payload.Position) && payload.Position != "-1")
                {
                    query = query.Where(w => w.PositionCode == payload.Position);
                }
                if (!string.IsNullOrWhiteSpace(payload.Department) && payload.Department != "-1")
                {
                    query = query.Where(w => w.DepartmentCode == payload.Department);
                }
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x => x.RealName.Contains(payload.Kw.Trim()) || x.Code.Contains(payload.Kw.Trim()));
                }
                if (payload.IsDeleted > CommonEnum.IsDeleted.All)
                {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (payload.Status > CommonEnum.Status.All)
                {
                    query = query.Where(x => x.Status == payload.Status);
                }
                var list = await query.Paged(payload.CurrentPage, payload.PageSize).ToListAsync();
                var totalCount = query.Count();
                var data = list.ToList();
                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建工资
        /// </summary>
        /// <param name="model">工资视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Create(WageCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            //if (model.RealName.Trim().Length <= 0)
            //{
            //    response.SetFailed("请输入工资名称");
            //    return Ok(response);
            //}
            await using (_dbContext)
            {
                if (_dbContext.ResumeInfo.Count(x => x.RealName == model.RealName) > 0)
                {
                    response.SetFailed("工资已存在");
                    return Ok(response);
                }
                var entity = _mapper.Map<WageCreateViewModel, WageInfo>(model);
                entity.CreatedOn = DateTime.Now;
                entity.Code = RandomHelper.GetRandomizer(8, true, false, true, true);
                entity.CreatedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.CreatedByUserName = AuthContextService.CurrentUser.DisplayName;

                await _dbContext.WageInfo.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                response.SetSuccess();
                return Ok(response);
            }
        }
        /// <summary>
        /// 导入薪资数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseModel>> Import()
        {
            var response = ResponseModelFactory.CreateInstance;
            //JArray lst = new JArray();
            //JObject jsonResult = new JObject();
            var lst = new List<WageInfo>();
            //long size = 0;
            var fileNamePath = string.Empty;
            var requestPath = string.Empty;

            try
            {
                await using (_dbContext)
                {
                    var files = Request.Form.Files;
                    foreach (var file in files)
                    {
                        ////string fileName = DateTime.Now.ToString("MMddHHmmss") + file.FileName;
                        //fileNamePath = DateTime.Now.ToString("MMddHHmmss") + file.FileName;
                        ////size += file.Length;

                        ////保存文件
                        ////物理路径 
                        //string SavePath = Path.Combine(CeyhConfiguration.TheUploadFileSettings.UploadFolder, fileNamePath);
                        ////using (FileStream fs = new FileStream(SavePath, FileMode.CreateNew))
                        ////{
                        ////    file.CopyTo(fs);
                        ////    fs.Flush();

                        ////}
                        //MemoryStream fs = new MemoryStream();
                        //file.CopyTo(fs);
                        DataTable dt = OfficeHelper.ReadStreamToDataTable(file.OpenReadStream());

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            WageInfo obj = new WageInfo
                            {
                                Status = CommonEnum.Status.Normal,
                                CreatedOn = DateTime.Now,
                                Code = RandomHelper.GetRandomizer(8, true, false, true, true),
                                CreatedByUserGuid = AuthContextService.CurrentUser.Guid,
                                CreatedByUserName = AuthContextService.CurrentUser.DisplayName,
                                RealName = dt.Rows[i][dt.Columns["真实姓名"]].ToString(),
                                DepartmentCode = _dbContext.UserDepartment
                                    .SingleOrDefault(s => s.Name == dt.Rows[i][dt.Columns["部门"]].ToString().Trim())
                                    ?.Code,
                                PositionCode = _dbContext.UserPosition.SingleOrDefault(s =>
                                    s.Name == dt.Rows[i][dt.Columns["职位"]].ToString().Trim())?.Code,
                                StartDate = DateTime.Parse(dt.Rows[i][dt.Columns["开始日期"]].ToString()
                                    ?.Replace("【", "")
                                    .Replace("】", "") ?? string.Empty),
                                EndDate = DateTime.Parse(dt.Rows[i][dt.Columns["结束日期"]].ToString()
                                    ?.Replace("【", "")
                                    .Replace("】", "") ?? throw new InvalidOperationException()),
                                TotalWage = decimal.Parse(dt.Rows[i][dt.Columns["应发工资"]].ToString() ?? string.Empty),
                                BaseWage = decimal.Parse(dt.Rows[i][dt.Columns["基本工资"]].ToString() ?? string.Empty),
                                WorkDays = int.Parse(dt.Rows[i][dt.Columns["工作天数"]].ToString() ?? throw new InvalidOperationException()),
                                OTWage = decimal.Parse(dt.Rows[i][dt.Columns["加班工资"]].ToString() ?? string.Empty),
                                OTDays = int.Parse(dt.Rows[i][dt.Columns["加班天数"]].ToString() ?? string.Empty),
                                PerformanceWage = decimal.Parse(dt.Rows[i][dt.Columns["绩效工资"]].ToString() ?? string.Empty),
                                ReissueWage = decimal.Parse(dt.Rows[i][dt.Columns["补发工资"]].ToString() ?? string.Empty),
                                Subsidy = decimal.Parse(dt.Rows[i][dt.Columns["补贴"]].ToString() ?? string.Empty),
                                Commission = decimal.Parse(dt.Rows[i][dt.Columns["提成"]].ToString() ?? string.Empty),
                                Bonus = decimal.Parse(dt.Rows[i][dt.Columns["奖金"]].ToString() ?? string.Empty),
                            };
                            //obj.StartDate = DateTime.Parse($"{dt.Rows[i][dt.Columns["开始日期"]].ToString().Split('-')[2]}-" +
                            //    $"{dt.Rows[i][dt.Columns["开始日期"]].ToString().Split('-')[1].Replace("月","")}-" +
                            //    $"{dt.Rows[i][dt.Columns["开始日期"]].ToString().Split('-')[0]}");
                            //obj.EndDate = DateTime.Parse($"{dt.Rows[i][dt.Columns["结束日期"]].ToString().Split('-')[2]}-" +
                            //    $"{dt.Rows[i][dt.Columns["结束日期"]].ToString().Split('-')[1].Replace("月", "")}-" +
                            //    $"{dt.Rows[i][dt.Columns["结束日期"]].ToString().Split('-')[0]}");


                            var adds = dt.Rows[i][dt.Columns["额外工资"]].ToString()?.Split('；');
                            obj.Additions = (adds ?? Array.Empty<string>()).Select(a => new { val = a.Split('：')[1], remark = a.Split('：')[0] }).ToJson();
                            obj.SocialSecurity = decimal.Parse(dt.Rows[i][dt.Columns["社保"]].ToString() ?? string.Empty);
                            obj.AccumulationFund = decimal.Parse(dt.Rows[i][dt.Columns["公积金"]].ToString() ?? string.Empty);
                            obj.IncomeTax = decimal.Parse(dt.Rows[i][dt.Columns["个税"]].ToString() ?? string.Empty);
                            obj.Deductions = dt.Rows[i][dt.Columns["额外扣除"]].ToString()
                                ?.Split('；').
                                Select(a => new { val = a.Split('：')[1], remark = a.Split('：')[0] }).ToJson();
                            //for (int j = 0; j < dt.Columns.Count; j++)
                            //{

                            //}
                            lst.Add(obj);
                        }
                        var count = lst.Count / 100 + 1;
                        for (var i = 0; i < count; i++)
                        {
                            var childish = lst.Skip(i).Take(1000).ToList();
                            await _dbContext.AddRangeAsync(childish);
                            await _dbContext.SaveChangesAsync();
                        }

                    }
                }

                //string url = Request.HttpContext.Connection.RemoteIpAddress.ToStringExt()+":"+ Request.HttpContext.Connection.RemotePort.ToStringExt();

            }
            catch (Exception ex)
            {
                response.SetError("上传文件错误信息列表错误：" + ex.ToString());
                return response;
            }
            return response;
        }

        private class JsonModel
        {
            public JsonModel(string val, string remark)
            {
                Val = val;
                Remark = remark;
            }

            public string Val { get; private set; }
            public string Remark { get; private set; }
        }
        /// <summary>
        /// 导出薪资列表
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<WageJsonModel>>>> 
            Export(WageExportPayload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;

            await using (_dbContext)
            {
                var query = (from wageInfo in _dbContext.WageInfo
                             join position in _dbContext.UserPosition on wageInfo.PositionCode equals position.Code
                             into t1
                             from position in t1.DefaultIfEmpty()
                             join department in _dbContext.UserDepartment on wageInfo.DepartmentCode equals department.Code
                             into t2
                             from department in t2.DefaultIfEmpty()
                             select new WageExportModel
                             {
                                 Code = wageInfo.Code,
                                 RealName = wageInfo.RealName,
                                 PositionCode = wageInfo.PositionCode,
                                 PositionName = position.Name,
                                 DepartmentCode = wageInfo.DepartmentCode,
                                 DepartmentName = department.Name,
                                 Start = wageInfo.StartDate,
                                 End = wageInfo.EndDate,
                                 StartDate = $"{((DateTime)wageInfo.StartDate):yyyy-MM-dd}",
                                 EndDate = $"{((DateTime)wageInfo.EndDate):yyyy-MM-dd}",
                                 BaseWage = wageInfo.BaseWage,
                                 WorkDays = wageInfo.WorkDays,
                                 OTDays = wageInfo.OTDays,
                                 OTWage = wageInfo.OTWage,
                                 PerformanceWage = wageInfo.PerformanceWage,
                                 ReissueWage = wageInfo.ReissueWage,
                                 Commission = wageInfo.Commission,
                                 Bonus = wageInfo.Bonus,
                                 SocialSecurity = wageInfo.SocialSecurity,
                                 Additions = wageInfo.Additions,
                                 //Additions= string.Join('；',JsonConvert.
                                 //DeserializeObject<List<JsonModel>>(wageInfo.Additions).
                                 //Select(s=>$"{s.remark}：{s.val}")),
                                 Subsidy = wageInfo.Subsidy,
                                 AccumulationFund = wageInfo.AccumulationFund,
                                 TotalWage = wageInfo.TotalWage,
                                 IncomeTax = wageInfo.IncomeTax,
                                 //Deductions = string.Join('；', JsonConvert.
                                 //DeserializeObject<List<JsonModel>>(wageInfo.Deductions).
                                 //Select(s => $"{s.remark}：{s.val}")),
                                 Deductions = wageInfo.Deductions,
                                 Status = wageInfo.Status,
                                 IsDeleted = wageInfo.IsDeleted,

                             });
                //取出数据源
                if (payload.Start.HasValue && payload.End.HasValue)
                {
                    query = query.Where(w => w.Start >= payload.Start && w.End <= payload.End);
                }
                if (!string.IsNullOrWhiteSpace(payload.realName))
                {
                    query = query.Where(w => w.RealName.Contains(payload.realName.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(payload.Position))
                {
                    query = query.Where(w => w.PositionCode == payload.Position);
                }
                if (!string.IsNullOrWhiteSpace(payload.Department))
                {
                    query = query.Where(w => w.DepartmentCode == payload.Department);
                }
                if (query.Count() > 300)
                {
                    response.SetFailed("导出数据过多请分批导出");
                    return Ok(response);
                }
                var list = await query.
                    Where(w => w.Status == CommonEnum.Status.Normal
                    && w.IsDeleted == CommonEnum.IsDeleted.No).ToListAsync();

                list.ForEach(r =>
                {
                    r.Additions = string.Join('；', JsonConvert.
                                 DeserializeObject<List<JsonModel>>(r.Additions).
                                 Select(s => $"{s.Remark}：{s.Val}"));
                    r.Deductions = string.Join('；', JsonConvert.
                                 DeserializeObject<List<JsonModel>>(r.Deductions).
                                 Select(s => $"{s.Remark}：{s.Val}"));
                });

                response.SetData(list);
                return Ok(response);


            }

            //return response;
        }

        /// <summary>
        /// 薪资报表
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResponseResultModel<IEnumerable<WageReport>>>> 
            Report(WageReportPayload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;

            var wageReports = new List<WageReport>();
            var colors = "0123456789ABCDEF".ToArray();
            await using (_dbContext)
            {
                var random = new Random();
                var query = _dbContext.WageInfo.Where(w => w.Status == CommonEnum.Status.Normal
                    && w.IsDeleted == CommonEnum.IsDeleted.No);
                //循环年份
                for (int i = 0; i < payload.Year; i++)
                {
                    var wageSeries = new List<WageSeries>();
                    var legend = new Legend { Data=new List<string>()};
                    var report = new WageReport();

                    switch (payload.Category)
                    {
                        case 0:
                            var series = new WageSeries
                            {
                                AreaStyle = new Areastyle
                                {
                                    Normal = new AreastyleNormal()
                                },
                                ItemStyle=new ItemStyle
                                {
                                    Normal=new ItemStyleNormal
                                    {
                                        Label=new ItemStyleLabel()
                                    }
                                }
                                
                            };
                            series.Name = "总体";
                            legend.Data.Add(series.Name);
                            var color = "#";
                            for (var j = 0; j < 6; j++)
                            {
                                color += colors[random.Next(0, 15)];
                            }
                            series.Data = GetData(query, i);
                            series.AreaStyle.Normal.Color = color;
                            wageSeries.Add(series);
                            break;
                        case 1:

                            (from p in _dbContext.WageInfo
                             group p by p.RealName into g
                             select new{g.Key}).ToList().ForEach(r =>{
                                 var queryTemp = query.Where(q => q.RealName == r.Key);
                                 var series = new WageSeries
                                 {
                                     AreaStyle = new Areastyle {Normal = new AreastyleNormal()},
                                     ItemStyle = new ItemStyle
                                     {
                                         Normal = new ItemStyleNormal {Label = new ItemStyleLabel()}
                                     },
                                     Name = r.Key
                                 };
                                 legend.Data.Add(series.Name);
                                 series.Data = GetData(queryTemp, i);
                                 var normalColor = "#";
                                 for (var j = 0; j < 6; j++)
                                 {
                                     normalColor += colors[random.Next(0, 15)];
                                 }
                                 series.AreaStyle.Normal.Color = normalColor;
                                 wageSeries.Add(series);
                             });
                            break;
                        case 2:

                            _dbContext.UserDepartment.ToList().ForEach(r =>
                            {
                                var queryTemp = query.Where(q => q.DepartmentCode.Trim() == r.Code.Trim());

                                var series = new WageSeries
                                {
                                    AreaStyle = new Areastyle
                                    {
                                        Normal = new AreastyleNormal()
                                    },
                                    ItemStyle = new ItemStyle
                                    {
                                        Normal = new ItemStyleNormal
                                        {
                                            Label = new ItemStyleLabel()
                                        }
                                    }
                                };
                                series.Name = r.Name;
                                legend.Data.Add(series.Name);
                                series.Data = GetData(queryTemp, i);
                                var normalColor = "#";
                                for (int j = 0; j < 6; j++)
                                {
                                    normalColor += colors[random.Next(0, 15)];
                                }
                                series.AreaStyle.Normal.Color = normalColor;
                                wageSeries.Add(series);
                            }
                                );
                            break;
                        case 3:

                            _dbContext.UserPosition.ToList().ForEach(r =>
                            {
                                var queryTemp = query.Where(q => q.PositionCode.Trim() == r.Code.Trim());
                                var series = new WageSeries
                                {
                                    AreaStyle = new Areastyle {Normal = new AreastyleNormal()},
                                    ItemStyle = new ItemStyle
                                    {
                                        Normal = new ItemStyleNormal {Label = new ItemStyleLabel()}
                                    },
                                    Name = r.Name
                                };
                                legend.Data.Add(series.Name);
                                series.Data = GetData(queryTemp, i);
                                var normalColor = "#";
                                for (int j = 0; j < 6; j++)
                                {
                                    normalColor += colors[random.Next(0, 15)];
                                }
                                series.AreaStyle.Normal.Color = normalColor;
                                wageSeries.Add(series);
                            }
                                );
                            break;
                        default:
                            break;
                    }

                    report.Title = $"{DateTime.Now.AddYears(-i):yyyy}年度薪资报表";
                    report.WageSeries = wageSeries;
                    report.Legend = legend;
                    wageReports.Add(report);
                }

                response.SetData(wageReports);

                static List<float> GetData(IQueryable<WageInfo> q, int year)
                {
                    var data = new List<float>();

                    for (var i = 0; i < 12; i++)
                    {
                        var start = DateTime.Parse(DateTime.Now.AddYears(-year).ToString("yyyy-01-01 00:00:00")).
                           AddMonths(i);
                        var end = DateTime.Parse(DateTime.Now.AddYears(-year).ToString("yyyy-01-01 00:00:00")).
                           AddMonths(i + 1);
                        // ReSharper disable once PossibleInvalidOperationException
                        data.Add((float)q.
                           Where(w => w.StartDate >= start
                            &&
                           w.EndDate < end)
                           .Sum(s => s.TotalWage));
                    }
                    return data;
                }

            }

            return Ok(response);
        }

        //[HttpPost]
        //public IActionResult Export()
        //{
        //    DateTime start = DateTime.MinValue;
        //    DateTime end = DateTime.MaxValue;
        //    var response = ResponseModelFactory.CreateInstance;

        //    using (_dbContext)
        //    {
        //        //取出数据源
        //        var result = _dbContext.WageInfo.
        //            Where(w => w.StartDate >= start && w.EndDate <= end
        //            && w.Status == CommonEnum.Status.Normal
        //            && w.IsDeleted == CommonEnum.IsDeleted.No).ToList();

        //        HSSFWorkbook book = new HSSFWorkbook();
        //        ISheet s1 = book.CreateSheet("薪资列表");
        //        //表格样式
        //        IDataFormat dataformat = book.CreateDataFormat();
        //        ICellStyle style0 = book.CreateCellStyle();
        //        style0.DataFormat = dataformat.GetFormat("yyyy年MM月dd日");


        //        IRow r2 = s1.CreateRow(1);

        //        r2.CreateCell(0).SetCellValue("真实姓名");
        //        r2.CreateCell(1).SetCellValue("部门");
        //        r2.CreateCell(2).SetCellValue("职位");
        //        r2.CreateCell(3).SetCellValue("开始日期");
        //        r2.CreateCell(4).SetCellValue("结束日期");
        //        r2.CreateCell(5).SetCellValue("应发工资");
        //        r2.CreateCell(6).SetCellValue("基本工资");
        //        r2.CreateCell(7).SetCellValue("工作天数");
        //        r2.CreateCell(8).SetCellValue("加班工资");
        //        r2.CreateCell(9).SetCellValue("加班天数");
        //        r2.CreateCell(10).SetCellValue("绩效工资");
        //        r2.CreateCell(11).SetCellValue("补贴");
        //        r2.CreateCell(12).SetCellValue("补发工资");
        //        r2.CreateCell(13).SetCellValue("提成");
        //        r2.CreateCell(14).SetCellValue("奖金");
        //        r2.CreateCell(15).SetCellValue("额外工资");
        //        r2.CreateCell(16).SetCellValue("社保");
        //        r2.CreateCell(17).SetCellValue("公积金");
        //        r2.CreateCell(18).SetCellValue("个税");
        //        r2.CreateCell(19).SetCellValue("额外扣除");

        //        int i = 0;
        //        for (; i < result.Count; i++)
        //        {
        //            NPOI.SS.UserModel.IRow rt = s1.CreateRow(i + 2);
        //            rt.CreateCell(0).SetCellValue(result[i].RealName);
        //            rt.CreateCell(1).SetCellValue(_dbContext.UserDepartment.Find(result[i].DepartmentCode)?.Name);
        //            rt.CreateCell(2).SetCellValue(_dbContext.UserPosition.Find(result[i].PositionCode)?.Name);
        //            rt.CreateCell(3).SetCellValue(((DateTime)result[i].StartDate).ToString("yyyy/MM/dd"));
        //            rt.CreateCell(4).SetCellValue(((DateTime)result[i].EndDate).ToString("yyyy/MM/dd")); rt.CreateCell(0).SetCellValue(result[i].RealName);
        //            rt.CreateCell(5).SetCellValue((double)result[i].TotalWage);

        //            rt.CreateCell(7).SetCellValue((double)result[i].WorkDays);
        //            rt.CreateCell(6).SetCellValue((double)result[i].BaseWage);
        //            rt.CreateCell(8).SetCellValue((double)result[i].OTWage);
        //            rt.CreateCell(9).SetCellValue((double)result[i].OTDays);
        //            rt.CreateCell(10).SetCellValue((double)result[i].PerformanceWage);
        //            rt.CreateCell(11).SetCellValue((double)result[i].Subsidy);
        //            rt.CreateCell(12).SetCellValue((double)result[i].ReissueWage);
        //            rt.CreateCell(13).SetCellValue((double)result[i].Commission);
        //            rt.CreateCell(14).SetCellValue((double)result[i].Bonus);
        //            rt.CreateCell(15).SetCellValue(result[i].Additions);
        //            rt.CreateCell(16).SetCellValue((double)result[i].SocialSecurity);
        //            rt.CreateCell(17).SetCellValue((double)result[i].AccumulationFund);
        //            rt.CreateCell(18).SetCellValue((double)result[i].IncomeTax);
        //            rt.CreateCell(19).SetCellValue(result[i].Deductions);


        //        }
        //        //NPOI.SS.UserModel.IRow rn = s1.CreateRow(i + 3);
        //        //输出的文件名称
        //        string fileName = "薪资列表" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff") + ".xls";
        //        //把Excel转为流，输出
        //        //创建文件流
        //        System.IO.MemoryStream bookStream = new System.IO.MemoryStream();
        //        //将工作薄写入文件流
        //        book.Write(bookStream);

        //        //输出之前调用Seek（偏移量，游标位置) 把0位置指定为开始位置
        //        bookStream.Seek(0, System.IO.SeekOrigin.Begin);
        //        //Stream对象,文件类型,文件名称
        //        return File(bookStream, "application/vnd.ms-excel;charset=UTF-8", fileName);
        //    }

        //    //return response;
        //}

        /// <summary>
        /// 编辑工资
        /// </summary>
        /// <param name="code">工资惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel<WageCreateViewModel>>> Edit(string code)
        {
            await using (_dbContext)
            {
                var entity = await _dbContext.WageInfo.FindAsync(code);
                var response = ResponseModelFactory.CreateInstance;
                var resEntity = _mapper.Map<WageInfo, WageCreateViewModel>(entity);
                response.SetData(resEntity);
                return Ok(response);
            }
        }


        /// <summary>
        /// 保存编辑后的工资信息
        /// </summary>
        /// <param name="model">角色视图实体</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Edit(WageCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (ConfigurationManager.AppSettings.IsTrialVersion)
            {
                response.SetIsTrial();
                return Ok(response);
            }

            await using (_dbContext)
            {
                if (await _dbContext.ResumeInfo.
                    CountAsync(x => x.RealName == model.RealName &&
                                    x.Code != model.Code) > 0)
                {
                    response.SetFailed("工资已存在");
                    return Ok(response);
                }

                var entity = _mapper.Map<WageCreateViewModel, WageInfo>(model);



                entity.ModifiedOn = DateTime.Now;
                entity.ModifiedByUserGuid = AuthContextService.CurrentUser.Guid;
                entity.ModifiedByUserName = AuthContextService.CurrentUser.DisplayName;

                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除工资
        /// </summary>
        /// <param name="ids">工资code,多个以逗号分隔</param>
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
            response =await UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
            return Ok(response);
        }

        /// <summary>
        /// 恢复工资
        /// </summary>
        /// <param name="ids">工资ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpPost("{ids}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ResponseModel>> Recover(string ids)
        {
            var response = await UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
            return Ok(response);
        }
        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">工资ID,多个以逗号分隔</param>
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
        #region 私有方法

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">角色ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private async Task<ResponseModel> UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids)
        {
            await using (_dbContext)
            {
                var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = $"UPDATE WageInfo SET IsDeleted=@IsDeleted WHERE Code IN ({parameterNames})";
                parameters.Add(new SqlParameter("@IsDeleted", (int)isDeleted));
                await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
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
        private async Task<ResponseModel> UpdateStatus(UserStatus status, string ids)
        {
            await using (_dbContext)
            {
                var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                var sql = $"UPDATE WageInfo SET Status=@Status WHERE Code IN ({parameterNames})";
                parameters.Add(new SqlParameter("@Status", (int)status));
                await _dbContext.Database.ExecuteSqlCommandAsync(sql, parameters);
                var response = ResponseModelFactory.CreateInstance;
                return response;
            }
        }
        #endregion
    }
}
