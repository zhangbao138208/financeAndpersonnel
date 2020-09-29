using DncZeus.Api.Cache;
using DncZeus.Api.Entities;
using DncZeus.Api.ViewModels.System.Dictionary;
using System.Collections.Generic;
using System.Linq;

namespace DncZeus.Api.Services
{
    public class DictionaryService
    {
        private readonly CacheData _cacheData;

        public DictionaryService(CacheData cacheData)
        {
            _cacheData = cacheData;
        }
        public DictionaryJsonModel GetSYSDictionary(string typeValue, string value)
        {
            var dicType = _cacheData.SysDictType.FirstOrDefault(t => t.Value == typeValue);
            var dic = _cacheData.SystemDictionary.FirstOrDefault(f => f.Value == value && f.TypeCode ==
              dicType?.Code
            );
            return dic;
        }

        // <summary>
        /// 字典
        /// </summary>
        /// <param name="DID"></param>
        /// <returns></returns>
        public  IEnumerable<DictionaryJsonModel> GetSYSDictionary(string typeValue)
        {
            var list = _cacheData.SystemDictionary.Where(f => f.TypeCode ==
             _cacheData.SysDictType.FirstOrDefault(t => t.Value == typeValue)?.Code);
            return list;
        }

        public void ClearDictionaryCache()
        {
            _cacheData.ClearDictionaryCache();
        }
    }
}
