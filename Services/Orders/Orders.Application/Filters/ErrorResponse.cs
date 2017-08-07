namespace Orders.Application.Filters
{
    public class ErrorResponse
    {
        public string[] Messages { get; set; }

        public object DeveloperMessage { get; set; }
    }
}