using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class SuscripcionServices
    {
        private readonly IMongoCollection<Suscripcion> _suscripciones;

        public SuscripcionServices(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _suscripciones = database.GetCollection<Suscripcion>("suscripciones");
        }

        public async Task<List<Suscripcion>> GetAllSuscripciones()
        {
            return await _suscripciones.Find(suscripcion => true).ToListAsync();
        }

        public async Task<Suscripcion> GetIdSuscripcion(string id)
        {
            return await _suscripciones.Find(suscripcion => suscripcion.Id == id).FirstOrDefaultAsync();
        }

        public async Task CrearSuscripcion(Suscripcion suscripcion)
        {
            await _suscripciones.InsertOneAsync(suscripcion);
        }

    }
}