using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Talent_Trade.Models
{
    public class Respuesta
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("idUser")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdUser { get; set; }

        [BsonElement("texto")]
        public string? Texto { get; set; }

        [BsonElement("fecha")]
        public DateTime? Fecha { get; set; }

        [BsonElement("likes")]
        public List<string>? Likes { get; set; }

    }
}