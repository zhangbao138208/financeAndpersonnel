using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Models
{
    public class RedisSettings
    {
        /// <summary>
        /// 链接
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// 缓存库,默认为0
        /// </summary>
        public int DefaultDatabase { get; set; } = 0;
    }
}
