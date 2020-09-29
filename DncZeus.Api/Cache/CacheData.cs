using AutoMapper;
using DncZeus.Api.Entities;
using DncZeus.Api.ViewModels.System.Dictionary;
using DncZeus.Api.ViewModels.System.DicType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Cache
{
    public class CacheData
    {
        private readonly DncZeusDbContext _dbContext;
        private readonly IMapper _mapper;

        public CacheData(DncZeusDbContext dbContext,IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public  List<DictionaryJsonModel> SystemDictionary
        {
            get
            {

                var list = CacheManager.GetCache<List<DictionaryJsonModel>>("DncZeusDic");
                if (list == null)
                {
                    //using (_dbContext)
                    //{
                    //    list = _dbContext.SystemDictionary.ToList();
                    //}
                    list = _dbContext.SystemDictionary.Select(_mapper.Map<SystemDictionary, DictionaryJsonModel>).ToList(); ;

                    CacheManager.SetCache("DncZeusDic", list);
                }
                return list;
            }
        }

        public List<DicTypeJsonModel> SysDictType
        {
            get
            {

                var list = CacheManager.GetCache<List<DicTypeJsonModel>>("DncZeusSysDictType");
                if (list == null)
                {
                    //using (_dbContext)
                    //{
                    //    list = _dbContext.SystemDicType.ToList();
                    //}
                    list = _dbContext.SystemDicType.Select(_mapper.Map<SystemDicType, DicTypeJsonModel>).ToList(); 

                    CacheManager.SetCache("DncZeusSysDictType", list);
                }
                return list;
            }
        }

        public void ClearDictionaryCache()
        {
            CacheManager.RemoveCache("DncZeusSysDictType");
            CacheManager.RemoveCache("DncZeusDic");
        }
    }
}
