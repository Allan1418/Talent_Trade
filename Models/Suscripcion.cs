using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Talent_Trade.Models
{
    public class Suscripcion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("idUser")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdUser { get; set; }

        [BsonElement("idCreador")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdCreador { get; set; }

        [BsonElement("fechaVencimiento")]
        public DateTime? FechaVencimiento { get; set; }
    }
}