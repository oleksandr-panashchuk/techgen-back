using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Models.Enums;

namespace Techgen.Domain.Entities.Identity
{
    public class UserChangeRequest : IEntity
    {
        #region Properties

        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public string TokenHash { get; set; }

        public ChangeRequestType ChangeRequestType { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty("UserChangeRequests")]
        public virtual ApplicationUser User { get; set; }

        #endregion
    }
}
