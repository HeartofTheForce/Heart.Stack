namespace Heart.Auth.Logic.Settings
{
    public class AuthSettings
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpireInMinutes { get; set; }
    }
}