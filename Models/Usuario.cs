
namespace Talent_Trade.Models
{
    using AspNetCore.Identity.Mongo.Model;
    using Microsoft.AspNetCore.Identity;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Usuario : MongoUser
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public override string? Id { get; set; }

        //[BsonElement("UserName")]
        //public override required string UserName { get; set; }

        //[BsonElement("email")]
        //public required string Email { get; set; }

        //[BsonElement("password")]
        //public required string Password { get; set; }

        [BsonElement("NombreCompleto")]
        public required string NombreCompleto { get; set; }

        [BsonElement("FechaRegistro")]
        public DateTime FechaRegistro { get; set; }

        [BsonElement("ImagePerfil")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ImagePerfil { get; set; }

        [BsonElement("IdDeCreador")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? IdDeCreador { get; set; }

        [BsonElement("Suscripciones")]
        public List<string>? Suscripciones { get; set; }

        [BsonElement("facturas")]
        public List<string>? Facturas { get; set; }
    }

}
