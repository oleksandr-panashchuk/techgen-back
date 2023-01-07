using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Entity;
using System.ComponentModel.DataAnnotations;
using Techgen.Domain.Extentions;

namespace Techgen.Domain.Entities
{
    [BsonCollection("profiles")]
    public class Profile : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonElement("Age")]
        public int Age { get; set; }

        [BsonElement("Country")]
        public string? Country { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        public DateTime CreatedAt => Id.CreationTime;
    }
}
