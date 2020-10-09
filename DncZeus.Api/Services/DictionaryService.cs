using DncZeus.Api.Cache;
using DncZeus.Api.ViewModels.System.Dictionary;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public DictionaryJsonModel GetSYSSeting(string name)
        {
            var dicType = _cacheData.SysDictType.FirstOrDefault(t => t.Value == "system");
            var dic = _cacheData.SystemDictionary.FirstOrDefault(f => f.Name == name && f.TypeCode ==
              dicType?.Code
            );
            return dic;
        }

        /// <summary>
        /// 异步获取
        /// </summary>
        /// <param name="typeValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<DictionaryJsonModel> GetSYSDictionaryAsync(string typeValue, string value)
        {
            var dicType = _cacheData.SysDictType.FirstOrDefault(t => t.Value == typeValue);
            var dic = _cacheData.SystemDictionary.FirstOrDefault(f => f.Value == value && f.TypeCode ==
              dicType?.Code
            );
            return await Task.FromResult<DictionaryJsonModel>(dic);
        }
        /// <summary>
        /// 同步获取
        /// </summary>
        /// <param name="typeValue"></param>
        /// <returns></returns>
        public  IEnumerable<DictionaryJsonModel> GetSYSDictionary(string typeValue)
        {
            var list = _cacheData.SystemDictionary.Where(f => f.TypeCode ==
             _cacheData.SysDictType.FirstOrDefault(t => t.Value == typeValue)?.Code);
            return list;
        }
        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="typeValue"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DictionaryJsonModel>> GetSYSDictionaryAsync(string typeValue)
        {
            var list = _cacheData.SystemDictionary.Where(f => f.TypeCode ==
             _cacheData.SysDictType.FirstOrDefault(t => t.Value == typeValue)?.Code);
            return await Task.FromResult<IEnumerable<DictionaryJsonModel>>(list);
        }

        public void ClearDictionaryCache()
        {
            _cacheData.ClearDictionaryCache();
        }
    }
}
