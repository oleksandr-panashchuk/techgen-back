using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Techgen.Domain.Entity;
using Techgen.Domain.Extentions;
using Realms;
using System.ComponentModel.DataAnnotations.Schema;

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

        [BsonElement("FirstName")]
        public string? FirstName { get; set; }

        [BsonElement("LastName")]
        public string? LastName { get; set; }

        [BsonElement("Age")]
        public int Age { get; set; }

        [BsonElement("Country")]
        public string? Country { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; }

        public DateTime CreatedAt => Id.CreationTime;

        #region Navigation properties
        [InverseProperty("Profile")]
        public User User { get; set; }
        #endregion

        #region Additional propeties
        [NotMapped]
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                    return $"{FirstName} {LastName}";
                else
                    return string.Empty;
            }
        }
        #endregion
    }
}
