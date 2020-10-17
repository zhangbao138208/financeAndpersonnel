using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Extensions.CustomException
{
    public class Forbidden:Exception
    {
        public Forbidden(string ms):base(ms)
        {
            
        }
    }
}
