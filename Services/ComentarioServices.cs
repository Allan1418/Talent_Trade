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
        public async Task<List<Comentario>> GetAllComentarios()
        {
            return await _comentarios.Find(comentario => true).ToListAsync();
        }

        public async Task<Comentario> GetIdComentario(string id)
        {
            return await _comentarios.Find(comentario => comentario.Id == id).FirstOrDefaultAsync();
        }

        public async Task CrearComentario(Comentario comentario)
        {
            await _comentarios.InsertOneAsync(comentario);
        }

    }
}