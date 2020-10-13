using System.Linq;
using System.Reflection;
using DncZeus.Api.Entities;

namespace DncZeus.Api.Extensions
{
    public static class DBsetExtensions
    {
        public static IQueryable<object> Set (this DncZeusDbContext _context, string name)
        {
            var  assembly=  Assembly.Load("DncZeus.Api");
            var implements = assembly.GetTypes().Where(w => w.IsClass);
            var t = implements.FirstOrDefault(i => i.Name == name);
            return (IQueryable<object>)_context.GetType().GetMethod("Set").MakeGenericMethod(t).Invoke(_context, null);
        }
    }
}