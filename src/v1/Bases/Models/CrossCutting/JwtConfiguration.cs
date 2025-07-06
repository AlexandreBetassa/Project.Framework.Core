namespace Store.Framework.Core.v1.Bases.Models.CrossCutting
{
    public class JwtConfiguration
    {
        public string SecretJwtKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int ExpirationInMinutes { get; set; }

        public string[] WriteRoles { get; set; }

        public string[] ReadRoles { get; set; }
    }
}