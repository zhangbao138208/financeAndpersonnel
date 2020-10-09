namespace DncZeus.Api.ViewModels.Workflow.Step
{
    public class StepSimpleJsomModel
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public bool IsCounterSign { get; set; }
        public string UserList { get; set; }
        public string[] Users { get; set; }
        public string[] UsersName { get; set; }
    }
}
