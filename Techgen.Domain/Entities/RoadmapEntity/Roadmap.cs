using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Domain.Entities.RoadmapEntity
{
    [CollectionName("FileModels")]
    public class Roadmap : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public ObjectId Id { get; set; }
        public string ImageName { get; set; }
        public string Path { get; set; }
        public string Markdown { get; set; }

        public DateTime CreatedAt => Id.CreationTime;
    }
}
