using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace AdvansioInteractive.Service.Internal.Lendng.Models
{
    public class BaseClass
    {
        [BsonId]
        [BsonElement("id")]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid Id { get; set; }
        [BsonElement("is_active")]
        public bool IsActive { get; set; }
        [BsonElement("is_deleted")]
        public bool IsDeleted { get; set; }
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("modified_at")]
        public DateTime ModifiedAt { get; set; }
        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonIgnore]
        [BsonElement("user_id")]
        public Guid UserId { get; set; } //the Identity ID of the AuthenticatedUser who created / owns this entity...
        public BaseClass()
        {
            CreatedAt = DateTime.Now;
            ModifiedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
