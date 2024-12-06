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
        public string IdCreador { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; }

        [BsonElement("descripcion")]
        public string Descripcion { get; set; }

        [BsonElement("precio")]
        public decimal Precio { get; set; } // Usamos decimal para representar valores monetarios con precisión
    }
}