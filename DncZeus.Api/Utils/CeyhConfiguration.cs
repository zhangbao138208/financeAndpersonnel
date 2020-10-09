using DncZeus.Api.Models;
using Microsoft.Extensions.Configuration;

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

        /// <summary>
        /// RSA
        /// </summary>
        public static RSASetting TheRSASetting => _Configuration.GetSection("RSASetting").Get<RSASetting>();
        #region 缓存配置

        /// <summary>
        /// 缓存类型，默认系统缓存
        /// </summary>
       // public static string TheCacheType => _Configuration.GetValue<string>("CacheType", "MemoryCache");
        public static string TheCacheType => _Configuration.GetValue<string>("CacheType", "Redis");

        /// <summary>
        /// Redis  Settings
        /// </summary>
        public static RedisSettings TheRedisSettings => _Configuration.GetSection("Redis").Get<RedisSettings>();

        #endregion


    }
}
