using DncZeus.Api.Extensions;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.User.Uploads;
using log4net.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Controllers.Api.V1
{
    [ApiController]
    [Authorize]
    public class CommonController:ControllerBase
    {
        

        #region 上传文件

        /// <summary>
        /// 上传文件 Form表单提交，
        /// appointName 指定文件名称
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload()
        {
            var response = ResponseModelFactory.CreateResultInstance;
            
            try
            {
                var result = new UploadJsonModel();
                if (Request.Form.Files.Count > 0)
                {
                    var upload = new UploadFile();
                    var appointFileName = string.Empty;
                    if (Request.Form.Keys.Contains("appointFileName"))
                    {
                        appointFileName = Request.Form["appointFileName"].ToString();
                    }
                    foreach (var file in Request.Form.Files)
                    {
                        upload.Save(file, appointFileName: appointFileName);
                    }

                    if (!upload.Error)
                    {
                        result.Msg = "上传成功";
                        result.Url = upload.FileInfo["filepath"].ToString();
                        result.Name = upload.FileInfo["Name"].ToString();

                        if (string.IsNullOrWhiteSpace(CeyhConfiguration.TheUploadFileSettings.HostUrl))
                        {
                            result.HostUrl = $"{Net.GetOrigin}{ result.Url}";
                        }
                        else
                        {
                            result.HostUrl = $"{CeyhConfiguration.TheUploadFileSettings.HostUrl}{ result.Url}";
                        }

                    }
                    else
                    {
                        result.Msg = $"上传失败，{upload.Message}";
                    }
                }
                else
                {
                    result.Msg = "未能获取到文件";
                }
                response.SetData(result);
                return Ok(response);
            }

            catch (Exception ex)
            {
                response.SetData($"消息[{ex.Message}]内容");
                return Ok(response);
            }

        }

        #endregion
    }
}
