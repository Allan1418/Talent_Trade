
namespace Talent_Trade.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Creador
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("idUser")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdUser { get; set; }

        [BsonElement("imageBackground")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ImageBackground { get; set; }

        [BsonElement("shortDescripcion")]
        public string? ShortDescripcion { get; set; }

        [BsonElement("acercaDe")]
        public string? AcercaDe { get; set; }

        [BsonElement("niveles")]
        public List<Nivel>? Niveles { get; set; }

        [BsonElement("ganancias")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Ganancias { get; set; }

        public class Nivel
        {
            [BsonElement("idNiveles")]
            [BsonRepresentation(BsonType.ObjectId)]
            public string? IdNiveles { get; set; }
        }
    }
}