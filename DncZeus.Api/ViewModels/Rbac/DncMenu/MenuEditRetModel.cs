using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.ViewModels.Rbac.DncMenu
{
    public class MenuEditRetModel
    {
        public MenuEditViewModel Model { get; set; }
        public List<MenuTree> Tree { get; set; }
    }
}
