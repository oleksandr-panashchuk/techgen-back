using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Techgen.Domain.Extentions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Techgen.Domain.Entities.Identity
{
    [BsonCollection("profiles")]
    public class Profile : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }

        [MaxLength(30)]
        public string FirstName { get; set; }

        [MaxLength(30)]
        public string LastName { get; set; }

        public int? AvatarId { get; set; }

        public string Country { get; set; }

        public int Age { get; set; }

        public DateTime CreatedAt => Id.CreationTime;

        #region Navigation properties
        [InverseProperty("Profile")]
        public virtual ApplicationUser User { get; set; }
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
