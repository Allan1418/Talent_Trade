using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class PublicacionServices
    {
        private readonly IMongoCollection<Publicacion> _publicaciones;
            
        public PublicacionServices(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _publicaciones = database.GetCollection<Publicacion>("publicaciones");
        }
        public async Task<List<Publicacion>> GetAllPublicaciones()
        {
            return await _publicaciones.Find(publicacion => true).ToListAsync();
        }

        public async Task<Publicacion> GetIdPublicacion(string id)
        {
            return await _publicaciones.Find(publicacion => publicacion.Id == id).FirstOrDefaultAsync();
        }

        public async Task CrearPublicacion(Publicacion publicacion)
        {
            await _publicaciones.InsertOneAsync(publicacion);
        }
    }
}