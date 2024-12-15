 using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Talent_Trade.Models
{
    public class Comentario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("idUser")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string IdUser { get; set; }
        
        [BsonElement("idPublicacion")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string IdPublicacion { get; set; }

        [BsonElement("texto")]
        public required string Texto { get; set; }

        [BsonElement("fecha")]
        public required DateTime Fecha { get; set; }

        [BsonElement("likes")]
        public List<string>? Likes { get; set; }

        [BsonIgnore]
        public string? FotoPerfil { get; set; }
        
        [BsonIgnore]
        public string? UserName { get; set; }
        
        [BsonIgnore]
        public List<Respuesta>? Respuestas { get; set; }

    }
}