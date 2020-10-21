using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DncZeus.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace DncZeus.Api.Services
{
    public class RoleService
    {
        private readonly DncZeusDbContext _dbContext;

        public RoleService(DncZeusDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<DncUser>> GetChildUserByUser(Guid guid)
        {
           // var uList = new List<DncUser> {user};
            var rList = await _dbContext.DncUserRoleMapping.Where(x =>
                string.Equals(x.UserGuid.ToString(), guid.ToString(), StringComparison.CurrentCultureIgnoreCase)).ToListAsync();
            var roleCodes = new List<string>();
            foreach (var roleMapping in rList)
            {
               await Find(roleMapping.RoleCode,roleCodes);  
                async Task Find(string code,ICollection<string> rl)
                {
                    var roles = await _dbContext.DncRole.
                        Where(x=>x.ParentCode==code).ToListAsync();
                    foreach (var role in roles.Where(role => role!=null))
                    {
                        rl.Add(role.Code);
                        await Find(role.Code,rl);
                    }
                }
            }

            var ss = await _dbContext.DncUserRoleMapping.ToListAsync();
            
            var mapUser=ss.Where(x => roleCodes.Contains(x.RoleCode,StringComparer.OrdinalIgnoreCase)).
                Select(x=>x.UserGuid).
                ToList();
            // var mapUser = await _dbContext.DncUserRoleMapping.
            //     Where(x => roleCodes.Contains(x.RoleCode,StringComparer.OrdinalIgnoreCase)).
            //     Select(x=>x.UserGuid).
            //     ToListAsync();
            var users = await _dbContext.DncUser.
                Where(x => mapUser.Contains(x.Guid)).
                ToListAsync();
            return users;
        }
    }
}
