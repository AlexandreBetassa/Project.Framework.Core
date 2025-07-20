namespace Project.Framework.Core.v1.Bases.Models.CrossCutting
{
    public class RedisConfiguration
    {
        public string Server { get; set; }

        public int ExpirationInMinutes { get; set; }
    }
}