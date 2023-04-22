using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Domain.Entities.Identity
{
    public class UserDevice : IEntity
    {
        #region Properties

        public int Id { get; set; }

        public int UserId { get; set; }

        public bool IsVerified { get; set; }

        public string DeviceToken { get; set; }

        public DateTime AddedAt { get; set; }

        public bool IsActive { get; set; }

        #endregion

        #region Navigation Properties

        [ForeignKey("UserId")]
        [InverseProperty("Devices")]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}
