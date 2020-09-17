using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.ViewModels.User.Uploads
{
    public class UploadJsonModel
    {
        /// <summary>
        /// 上传文件消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 域名地址
        /// </summary>
        public string HostUrl { get; set; }
    }
}
