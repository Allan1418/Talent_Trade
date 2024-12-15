
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
        public required string IdUser { get; set; }

        [BsonElement("UserName")]
        public required string UserName { get; set; }
        
        [BsonElement("nombrePagina")]
        public required string nombrePagina { get; set; }

        [BsonElement("imageBackground")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ImageBackground { get; set; }

        [BsonElement("shortDescripcion")]
        public required string ShortDescripcion { get; set; }

        [BsonElement("acercaDe")]
        public required string AcercaDe { get; set; }

        //[BsonElement("niveles")]
        //public List<string>? Niveles { get; set; }

        //[BsonElement("ganancias")]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string? Ganancias { get; set; }
    }
}