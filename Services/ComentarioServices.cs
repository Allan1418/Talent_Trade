using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class ComentarioServices
    {
        private readonly IMongoCollection<Comentario> _comentarios;

        public ComentarioServices(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _comentarios = database.GetCollection<Comentario>("comentarios");
        }

        public List<Comentario> GetAll()
        {
            return _comentarios.Find(comentarios => true).ToList();
        }

        public Comentario Get(string id)
        {
            return _comentarios.Find<Comentario>(comentarios => comentarios.Id == id).FirstOrDefault();
        }

        public Comentario Create(Comentario comentarios)
        {
            _comentarios.InsertOne(comentarios);
            return comentarios;
        }

        public void Update(string id, Comentario comentarioIn) =>
            _comentarios.ReplaceOne(comentario => comentario.Id == id, comentarioIn);

        public void Remove(Comentario comentarioIn) =>
            _comentarios.DeleteOne(comentario => comentario.Id == comentarioIn.Id);

        public void Remove(string id) =>
            _comentarios.DeleteOne(comentario => comentario.Id == id);
    }
}