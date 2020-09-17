using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.ViewModels.User.Position
{
    public class PositionJsonModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentCode { get; set; }
        public int LevelID { get; set; }
        public int SortID { get; set; }
        public Status Status { get; set; }
        public string CreatedOn { get; set; }
        public Guid CreatedByUserGuid { get; set; }
        public string CreatedByUserName { get; set; }
        public string ModifiedOn { get; set; }
        public Guid? ModifiedByUserGuid { get; set; }
        public string ModifiedByUserName { get; set; }
    }
}
