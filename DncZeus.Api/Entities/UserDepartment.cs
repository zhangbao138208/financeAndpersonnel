using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Entities
{
    /// <summary>
    /// 用户部门实体类
    /// </summary>
   
    public class UserDepartment: Entity
    {
        [Required]
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ParentCode { get; set; }
        public int LevelID { get; set; }
        public int SortID { get; set; }
        public int TypeID { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Zone { get; set; }
        public string Manager { get; set; }
        public string Company { get; set; }
       
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
    }

    
}
