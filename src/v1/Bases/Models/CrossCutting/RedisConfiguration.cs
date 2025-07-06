namespace Store.Framework.Core.v1.Bases.Models.CrossCutting
{
    public class RedisConfiguration
    {
        internal string Server { get; set; }

        public int ExpirationInMinutes { get; set; }
    }
}