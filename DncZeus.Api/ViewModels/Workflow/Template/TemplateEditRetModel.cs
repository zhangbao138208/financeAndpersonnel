using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DncZeus.Api.ViewModels.Workflow.Template
{
    public class TemplateEditRetModel
    {
        public TemplateEditViewModel Model { get; set; }
        public List<TemplateTree> Tree { get; set; }
    }
}
