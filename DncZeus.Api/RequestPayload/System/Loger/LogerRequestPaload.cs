using System;

namespace DncZeus.Api.RequestPayload.System.Loger
{
    public class LogerRequestPaload:RequestPayload
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}