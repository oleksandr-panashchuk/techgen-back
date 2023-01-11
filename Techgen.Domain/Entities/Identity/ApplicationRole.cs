using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techgen.Domain.Extentions;

namespace Techgen.Domain.Entities.Identity
{
    public class ApplicationRole : MongoIdentityRole, IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }

        public DateTime CreatedAt => Id.CreationTime;

        public ApplicationRole()
        {
            UserRoles = new List<ApplicationUserRole>();
        }
    }
}
