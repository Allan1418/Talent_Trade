using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Talent_Trade.Models
{
    public class MesGanancia
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("idCreador")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string IdCreador { get; set; }

        [BsonElement("month")]
        [BsonRepresentation(BsonType.Int32)]
        public required int Month { get; set; }

        [BsonElement("year")]
        [BsonRepresentation(BsonType.Decimal128)]
        public required int Year { get; set; }

        [BsonElement("total")]
        [BsonRepresentation(BsonType.Decimal128)]
        public required decimal Total { get; set; }
    }
}
