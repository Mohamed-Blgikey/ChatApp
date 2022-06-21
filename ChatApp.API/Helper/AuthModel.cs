namespace ChatApp.API.Helper
{
    public class AuthModel
    {
        public string Message { get; set; }
        public string Token { get; set; }
        public bool IsAuthencated { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
