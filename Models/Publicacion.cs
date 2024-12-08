﻿using MongoDB.Bson;
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
        public List<string> Likes { get; set; }

        [BsonElement("tier")]
        public int Tier { get; set; }

        [BsonElement("comentarios")]
        public List<string> Comentarios { get; set; }

        [BsonElement("adjuntos")]
        public List<string> Adjuntos { get; set; }

    }
}