using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using Techgen.Domain.Extentions;

namespace Techgen.Domain.Entity
{
    [BsonCollection("users")]
    public class User : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; }

        public DateTime CreatedAt => Id.CreationTime;
    }
}
