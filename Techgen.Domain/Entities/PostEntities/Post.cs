using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using Techgen.Domain.Entities.Identity;

namespace Techgen.Domain.Entities.PostEntities
{
    public class Post : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }

        [BsonElement("AuthorId")]
        [BsonRepresentation(BsonType.String)]
        public ObjectId UserId { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        #region Navigation properties

        [InverseProperty("Post")]    
        public virtual ICollection<Comment> Comments { get; set; }

        [InverseProperty("Post")]
        public virtual ICollection<Like> Likes { get; set; }

        [InverseProperty("Posts")]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}
