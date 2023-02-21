using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techgen.Domain.Entities.Identity;

namespace Techgen.Domain.Entities.PostEntities
{
    public class Post : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        
        [DefaultValue("")]
        public string Title { get; set; }

        [DefaultValue("")]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; }

        #region Navigation properties

        [InverseProperty("Post")]    
        public virtual ICollection<Comment> Comments { get; set; }

        [InverseProperty("Post")]
        public virtual ICollection<Like> Likes { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Posts")]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}
