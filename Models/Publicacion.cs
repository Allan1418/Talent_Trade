using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Talent_Trade.Models
{
    public class Publicacion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("idCreador")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IdCreador { get; set; }

        [BsonElement("titulo")]
        public string Titulo { get; set; }

        [BsonElement("contenido")]
        public string Contenido { get; set; }

        [BsonElement("fecha")]
        public DateTime Fecha { get; set; }

        [BsonElement("likes")]
        public List<Like> Likes { get; set; }

        [BsonElement("tier")]
        public int Tier { get; set; }

        [BsonElement("comentarios")]
        public List<Comentario> Comentarios { get; set; }

        [BsonElement("adjuntos")]
        public List<Multimedia> Adjuntos { get; set; }

        public class Like
        {
            [BsonElement("idUser")]
            [BsonRepresentation(BsonType.ObjectId)]
            public string IdUser { get; set; }
        }

        public class Comentario
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)]
            public string IdComentario { get; set; }

            [BsonElement("idUsuario")]
            [BsonRepresentation(BsonType.ObjectId)]
            public string IdUsuario { get; set; }
        }

        public class Multimedia
        {
            [BsonElement("url")]
            public string Url { get; set; }
        }
    }
}