using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Models
{
    public class UploadFileSettings
    {
        /// <summary>
        /// 文件上传根目录
        /// </summary>
        public string UploadRoot { get; set; }

        /// <summary>
        /// 系统文件夹
        /// </summary>
        public string SystemFolder { get; set; }

        /// <summary>
        /// 上传文件夹
        /// </summary>
        public string UploadFolder { get; set; }

        /// <summary>
        /// 下载文件夹
        /// </summary>
        public string DownloadFolder { get; set; }

        /// <summary>
        /// 主机服务地址
        /// </summary>
        public string HostUrl { get; set; }
    }
}
