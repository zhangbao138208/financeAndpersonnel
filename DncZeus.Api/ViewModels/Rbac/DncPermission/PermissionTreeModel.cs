using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.ViewModels.Rbac.DncPermission
{
    public class PermissionTreeModel
    {
        public List<String> SelectedPermissions { get; set; }
        public List<PermissionMenuTree> Tree { get; set; }
    }
}
