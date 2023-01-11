using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techgen.Domain.Entities.PostEntities;
using Techgen.Domain.Extentions;

namespace Techgen.Domain.Entities.Identity
{
    [BsonCollection("users")]
    public class ApplicationUser : MongoIdentityUser<Guid>, IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [DataType("DateTime")]
        public DateTime RegistratedAt { get; set; }

        [DataType("DateTime")]
        public DateTime? DeletedAt { get; set; }

        [DataType("DateTime")]
        public DateTime? LastVisitAt { get; set; }

        [BsonElement("RecoveryCode")]
        public string RecoveryCode { get; set; }

        [BsonElement("DigitId")]
        public string DigitId { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt => Id.CreationTime;

        #region Navigation properties

        [InverseProperty("User")]
        public virtual ICollection<Comment> Comments { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Post> Posts { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Like> Likes { get; set; }

        [InverseProperty("User")]
        public virtual Profile Profile { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserToken> Tokens { get; set; }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<VerificationToken> VerificationTokens { get; set; }


        #endregion

        public ApplicationUser()
        {
            Tokens = new List<UserToken>();
            UserRoles = new List<ApplicationUserRole>();
            VerificationTokens = new List<VerificationToken>();
            Posts = new List<Post>();
            Comments = new List<Comment>();
            Likes = new List<Like>();
        }
    }
}
