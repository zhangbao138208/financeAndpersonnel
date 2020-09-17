using DncZeus.Api.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Utils
{
    /// <summary>
    /// 配置信息
    /// </summary>
    public class CeyhConfiguration
    {
        private static IConfiguration _Configuration;

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="configuration"></param>
        public static void Instance(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        
        /// <summary>
        /// 文件
        /// </summary>
        public static UploadFileSettings TheUploadFileSettings => _Configuration.GetSection("UploadFileSettings").Get<UploadFileSettings>();


       
    }
}
