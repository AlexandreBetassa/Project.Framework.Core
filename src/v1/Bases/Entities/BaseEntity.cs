using System.Text.Json.Serialization;

namespace Project.Framework.Core.v1.Bases.Entities
{
    public class BaseEntity
    {
        [JsonIgnore]
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool Status { get; set; }
    }
}