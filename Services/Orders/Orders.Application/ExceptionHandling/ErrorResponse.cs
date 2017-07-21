namespace Orders.Application.ExceptionHandling
{
    public class ErrorResponse
    {
        public string[] Messages { get; set; }

        public object DeveloperMessage { get; set; }
    }
}