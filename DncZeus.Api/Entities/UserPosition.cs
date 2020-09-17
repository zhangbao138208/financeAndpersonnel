using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.Entities
{
    public class UserPosition:Entity
    {
        [Required]
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentCode { get; set; }
        public int LevelID { get; set; }
        public int SortID { get; set; }
    }
}
