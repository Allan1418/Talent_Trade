using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Talent_Trade.Models
{
    public class Ganancia
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("idCreador")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string IdCreador { get; set; }

        [BsonElement("sinRetirar")]
        [BsonRepresentation(BsonType.Decimal128)]
        public required decimal SinRetirar { get; set; }

        [BsonElement("retirado")]
        [BsonRepresentation(BsonType.Decimal128)]
        public required decimal Retirado { get; set; }

        [BsonElement("total")]
        [BsonRepresentation(BsonType.Decimal128)]
        public required decimal Total { get; set; }
    }
}
