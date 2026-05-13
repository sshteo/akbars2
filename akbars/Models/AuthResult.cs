namespace akbars.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public SessionContext Session { get; set; }
    }
}
