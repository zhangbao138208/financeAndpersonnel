using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.RequestPayload.Wage
{
    public class WageExportPayload
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string realName { get; set; }
    }
}
