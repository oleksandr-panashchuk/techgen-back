using AspNetCore.Identity.MongoDbCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Extentions;

namespace Techgen.Domain.Entities.Identity
{
    [BsonCollection("userRoles")]
    public class ApplicationUserRole : MongoIdentityRole
    {
        public virtual ApplicationUser User { get; set; }

        public virtual ApplicationRole Role { get; set; }
    }
}
