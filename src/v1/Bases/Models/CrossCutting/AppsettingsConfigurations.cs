namespace Store.Framework.Core.v1.Bases.Models.CrossCutting
{
    public class AppsettingsConfigurations
    {
        public JwtConfiguration JwtConfiguration { get; set; }

        public RedisConfiguration RedisConfiguration { get; set; }

        public string Database { get; set; }
    }
}
