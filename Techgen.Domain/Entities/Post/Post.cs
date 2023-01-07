using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Realms;
using Realms.Schema;
using Realms.Weaving;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Entity;

namespace Techgen.Domain.Entities.Post
{
    public class Post : IEntity
    {
        [BsonId]
        [PrimaryKey]
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

        [Backlink("Post")]    
        public IQueryable<Comment>? Comments { get; set; }

        [Backlink("Posts")]
        public User User { get; set; }

        #endregion
    }
}
