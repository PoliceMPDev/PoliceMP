namespace PoliceMP.Shared.Results
{
    public class ActionResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static ActionResult Success(string message = "")
            => new ActionResult { IsSuccess = true, Message = message };

        public static ActionResult Fail(string message = "")
            => new ActionResult { IsSuccess = false, Message = message };
    }
}
