using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Domain.Entities.Identity
{
    public class UserToken : IEntity
    {
        #region Properties

        public int Id { get; set; }

        public int UserId { get; set; }

        [MaxLength(200)]
        [DefaultValue("")]
        public string AccessTokenHash { get; set; }

        [MaxLength(200)]
        [DefaultValue("")]
        public string RefreshTokenHash { get; set; }

        public DateTime AccessExpiresDate { get; set; }

        public DateTime RefreshExpiresDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsActive { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("UserId")]
        [InverseProperty("Tokens")]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}
