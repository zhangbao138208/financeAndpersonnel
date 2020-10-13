namespace DncZeus.Api.ViewModels.System.Loger
{
    public class SystemLogJsonModel
    {
        public string Application { get; set; }
        public string Levels { get; set; }
        public string Operatingtime { get; set; }
        public string Operatingaddress { get; set; }
        public string Logger { get; set; }
        public string Callsite { get; set; }
        public string Requesturl { get; set; }
        public string Referrerurl { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}