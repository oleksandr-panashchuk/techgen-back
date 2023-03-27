using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techgen.Common.Extensions;
using Techgen.Domain.Entities.PostEntities;

namespace Techgen.Domain.Entities.Identity
{
    public partial class ApplicationUser : IdentityUser<int>, IEntity
    {
        #region Properties

        [Key]
        public override int Id { get; set; }

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

        /// <summary>
        /// Difference between server and user timezones in hours 
        /// </summary>
        public double TimeZoneOffset { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty("User")]
        public virtual Profile Profile { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserToken> Tokens { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<VerificationToken> VerificationTokens { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Comment> Comments { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Post> Posts { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Like> Likes { get; set; }

        #endregion

        #region Additional Properties

        [NotMapped]
        public DateTime ClientTime
        {
            get
            {
                return DateTime.UtcNow.AddHours(TimeZoneOffset);
            }
        }

        #endregion

        #region Ctors

        public ApplicationUser()
        {
            Tokens = Tokens.Empty();
            UserRoles = UserRoles.Empty();
            VerificationTokens = VerificationTokens.Empty();
            Posts = Posts.Empty();
            Comments = Comments.Empty();
            Likes = Likes.Empty();
        }

        #endregion
    }
}
