using DncZeus.Api.Entities;
using System;

namespace DncZeus.Api.Extensions
{
    public class WorkflowReceiverFactory
    {
        public static WorkflowReceiver CreateInstance => new WorkflowReceiver { 
            CreateDate=DateTime.Now,
            Status="0",
            IsCheck=false
        };
    }
}
