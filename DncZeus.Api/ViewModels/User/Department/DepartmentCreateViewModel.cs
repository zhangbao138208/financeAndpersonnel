using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DncZeus.Api.Entities.Enums.CommonEnum;

namespace DncZeus.Api.ViewModels.User.Department
{
    public class DepartmentCreateViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int SortID { get; set; }
        public Status Status { get; set; }
        public IsDeleted IsDeleted { get; set; }

        public string WorkTime { get; set; }
        public string[] RestDays { get; set; }

    }
}
