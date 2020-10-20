using DncZeus.Api.Extensions;
using DncZeus.Api.Utils;
using DncZeus.Api.ViewModels.User.Uploads;
using log4net.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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

                        result.HostUrl = string.IsNullOrWhiteSpace(CeyhConfiguration.TheUploadFileSettings.HostUrl) ? $"{Net.GetOrigin}{ result.Url}" : $"{CeyhConfiguration.TheUploadFileSettings.HostUrl}{ result.Url}";

                    }
                    else
                    {
                        //result.Msg = $"上传失败，{upload.Message}";
                        response.SetError($"上传失败，{upload.Message}");
                        return Ok(response);
                    }
                }
                else
                {
                    //result.Msg = "未能获取到文件";
                    response.SetError($"未能获取到文件");
                    return Ok(response);
                }
                response.SetData(result);
                return Ok(response);
            }

            catch (Exception ex)
            {
                response.SetError($"消息[{ex.Message}]内容");
                return Ok(response);
            }

        }
        /// <summary>
        /// 移除文件
        /// </summary>
        /// <param name="urls"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("FileDelete")]
        public async Task<IActionResult> FileDelete(List<string> urls)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            var r = urls[0].Replace(CeyhConfiguration.TheUploadFileSettings.HostUrl,  Directory.GetCurrentDirectory());
            foreach (var url in urls.Where(url =>
              
                global::System.IO.File.Exists(url.Replace(CeyhConfiguration.TheUploadFileSettings.HostUrl, Directory.GetCurrentDirectory()))))
            {
                // Use a try block to catch IOExceptions, to
                // handle the case of the file already being
                // opened by another process.
                try
                {
                    global::System.IO.File.Delete(url.Replace(CeyhConfiguration.TheUploadFileSettings.HostUrl, Directory.GetCurrentDirectory()));
                    
                }
                catch (IOException e)
                {
                   response.SetError(e.Message);
                }
            }

            return Ok(response);
        }

        // [AllowAnonymous]
        [HttpPost]
        [Route("EditorUpload")]
        public async Task<IActionResult> EditorUpload()
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

                        result.HostUrl = string.IsNullOrWhiteSpace(CeyhConfiguration.TheUploadFileSettings.HostUrl) ? $"{Net.GetOrigin}{ result.Url}" : $"{CeyhConfiguration.TheUploadFileSettings.HostUrl}{ result.Url}";

                    }
                    else
                    {
                        //result.Msg = $"上传失败，{upload.Message}";
                        // response.SetError($"上传失败，{upload.Message}");
                        return Ok(new { error = 1, message = $"上传失败，{upload.Message}" });
                    }
                }
                else
                {
                    //result.Msg = "未能获取到文件";
                    //response.SetError($"未能获取到文件");
                    //return Ok(response);
                    return Ok(new { error = 1, message = "未能获取到文件" });

                }
                //response.SetData(result);
                //return Ok(response);
                return Ok(new { error = 0, url = result.HostUrl });
            }

            catch (Exception ex)
            {
                //response.SetError($"消息[{ex.Message}]内容");
                //return Ok(response);
                return Ok(new { error = 1, message = $"消息[{ex.Message}]内容" });
            }

        }

        #endregion
    }
}
