using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Entities.Identity;
using Techgen.Domain.Extentions;

namespace Techgen.Domain.Entities.PostEntities
{
    [BsonCollection("likes")]
    public class Like : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }

        public string UserId { get; set; }

        public string PostId { get; set; }

        public DateTime CreatedAt => Id.CreationTime;

        #region Navigation properties
        [InverseProperty("Likes")]
        public virtual ApplicationUser User { get; set; }

        [InverseProperty("Likes")]
        public virtual Post Post { get; set; }
        #endregion

    }
}
