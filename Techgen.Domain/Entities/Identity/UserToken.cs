using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Extentions;

namespace Techgen.Domain.Entities.Identity
{
    [BsonCollection("userTokens")]
    public class UserToken : IEntity
    {
        #region Properties

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }

        public string UserId { get; set; }

        [MaxLength(200)]
        public string AccessTokenHash { get; set; }

        [MaxLength(200)]
        public string RefreshTokenHash { get; set; }

        public DateTime AccessExpiresDate { get; set; }

        public DateTime RefreshExpiresDate { get; set; }

        public DateTime CreatedAt => Id.CreationTime;

        public bool IsActive { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty("Tokens")]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}
