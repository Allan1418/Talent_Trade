using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Talent_Trade.Models
{
    public class NivelSuscripcion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("idCreador")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string IdCreador { get; set; }

        [BsonElement("nombre")]
        public required string Nombre { get; set; }

        [BsonElement("descripcion")]
        public required string Descripcion { get; set; }

        [BsonElement("precio")]
        [BsonRepresentation(BsonType.Decimal128)]
        public required decimal Precio { get; set; }
    }
}