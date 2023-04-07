using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Entities.Identity;

namespace Techgen.Domain.Entities.PostEntities
{
    public class Like : IEntity<int>
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PostId { get; set; }

        public DateTime CreatedAt { get; set; }

        #region Navigation properties
        [ForeignKey("UserId")]
        [InverseProperty("Likes")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("PostId")]
        [InverseProperty("Likes")]
        public virtual Post Post { get; set; }
        #endregion

    }
}
