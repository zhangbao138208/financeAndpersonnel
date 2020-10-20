using System;
using System.Collections.Generic;

namespace DncZeus.Api.ViewModels.Workflow.Receiver
{
    public class ReceiverReadOnlyModel
    {
        public int Id { get; set; }
        public string WorkflowCode { get; set; }
        public string WorkflowName { get; set; }

        public List<Note> Notes { get; set; }
        public List<string> Steps { get; set; }
        public int CurrentStep { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public Guid CreateUser { get; set; }
        public string CreateUserName { get; set; }
        public string DateSpan { get; set; }
        public string Additions { get; set; }
        
    }
    public class Note
    {
        public string NodeName { get; set; }
        public string Status { get; set; }
        public string StatusName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string UserName { get; set; }
        public string Opinion { get; set; }
        public string NodeDate { get; set; }
    }
}
