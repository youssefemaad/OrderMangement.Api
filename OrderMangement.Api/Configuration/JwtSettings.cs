namespace OrderMangement.Api.Configuration
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public int ExpiryInDays { get; set; }
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
