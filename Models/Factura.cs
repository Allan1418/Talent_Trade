using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Talent_Trade.Models
{
    public class Factura
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("idUser")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string IdUser { get; set; }

        [BsonElement("idCreador")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string IdCreador { get; set; }

        [BsonElement("monto")]
        public required decimal Monto { get; set; }

        [BsonElement("fechaPago")]
        public required DateTime FechaPago { get; set; }
    }
}