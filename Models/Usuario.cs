
namespace Talent_Trade.Models
{

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombreCompleto")]
        public required string NombreCompleto { get; set; }

        [BsonElement("userName")]
        public required string UserName { get; set; }

        [BsonElement("email")]
        public required string Email { get; set; }

        [BsonElement("password")]
        public required string Password { get; set; }

        [BsonElement("fechaRegistro")]
        public DateTime FechaRegistro { get; set; }

        [BsonElement("ImagePerfil")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ImagePerfil { get; set; }

        [BsonElement("idDeCreador")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdDeCreador { get; set; }

        [BsonElement("suscripciones")]
        public List<string>? Suscripciones { get; set; }

        [BsonElement("facturas")]
        public List<string>? Facturas { get; set; }
    }

}
