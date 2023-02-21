using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Domain.Entities.RoadmapEntity
{
    public class Roadmap : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string Path { get; set; }
        public string Markdown { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
