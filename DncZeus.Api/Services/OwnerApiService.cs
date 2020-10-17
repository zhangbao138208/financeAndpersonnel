using System.Collections.Generic;
using System.Threading.Tasks;
using DncZeus.Api.Cache;
using DncZeus.Api.Extensions.CustomException;

namespace DncZeus.Api.Services
{
    public class OwnerApiService
    {
        private readonly CacheData _cacheData;
        public OwnerApiService(CacheData cacheData)
        {
            _cacheData = cacheData;
        }
        public async Task<OwnedApiPermission> GetApiEntry()
        {
            var ret = _cacheData.ApiEntry;
            
            return await Task.FromResult<OwnedApiPermission>(ret);
        }
        
        public void ClearCache(List<string> users) 
        {
            _cacheData.ClearApiEntryCache(users);
        }
    }
}