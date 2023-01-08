using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Entity;
using Techgen.Domain.Extentions;

namespace Techgen.Domain.Entities.Post
{
    [BsonCollection("comments")]
    public class Comment : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }
        
        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        public ObjectId? ParentCommentId { get; set; }

        #region Navigation properties

        public ICollection<Comment>? Answers { get; set; }

        [Backlink("Comments")]
        public User User { get; set; }

        [Backlink("Comments")]
        public Post Post { get; set; }

        #endregion
    }
}
