using DncZeus.Api.Utils;
using DncZeus.Api.Utils.Encryption;
using System;
using System.Linq;

namespace DncZeus.Api.Cache
{
    public class CacheManager
    {
        /// <summary>
        /// 版本
        /// </summary>
        private static string Version { get; set; }

        private static ICacheHelper _cache
        {
            get
            {
                string cachetpye = CeyhConfiguration.TheCacheType;
                if ("Redis".Equals(cachetpye, StringComparison.CurrentCultureIgnoreCase))
                {
                    return SingletonProvider<RedisCacheHelper>.Instance;
                }
                else
                {
                    return SingletonProvider<MemoryCacheHelper>.Instance;
                }
            }
        }

        public static bool Exists(string key)
        {
            return _cache.Exists(key);
        }

        public static T GetCache<T>(string key) where T : class
        {
            return _cache.GetCache<T>(key);
        }

        public static void SetCache(string key, object value)
        {
            _cache.SetCache(key, value);
        }

        public static string GetCacheKey(string url, params object[] paraList)
        {
            if (paraList == null)
            {
                return MD5.Encrypt($"{url}");
            }
            var strArray = paraList.Where(x => x != null)
                                   .Select(s => {
                                       return s.ToJson();
                                   });
            return MD5.Encrypt($"{url}_PARAMS_{string.Join("_", strArray)}_{Version}");
        }

        public static void SetCache(string key, object value, DateTimeOffset expiressAbsoulte)
        {
            _cache.SetCache(key, value, expiressAbsoulte);
        }

        public static void RemoveCache(string key)
        {
            _cache.RemoveCache(key);
        }
    }
}
