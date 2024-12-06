using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class NivelSuscripcionServices
    {
        private readonly IMongoCollection<NivelSuscripcion> _nivelesSuscripciones;

        public NivelSuscripcionServices(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _nivelesSuscripciones = database.GetCollection<NivelSuscripcion>("nivelesSuscripciones");
        }
        public async Task<List<NivelSuscripcion>> GetAllNivelesSuscripcione()
        {
            return await _nivelesSuscripciones.Find(nivel => true).ToListAsync();
        }

        public async Task<NivelSuscripcion> GetIdNivelSuscripcion(string id)
        {
            return await _nivelesSuscripciones.Find(nivel => nivel.Id == id).FirstOrDefaultAsync();
        }

        public async Task CrearNivelSuscripcion(NivelSuscripcion nivelSuscripcion)
        {
            await _nivelesSuscripciones.InsertOneAsync(nivelSuscripcion);
        }

    }
}