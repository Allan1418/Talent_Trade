using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class RespuestaServices
    {
        private readonly IMongoCollection<Respuesta> _respuestas;
        public RespuestaServices(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _respuestas = database.GetCollection<Respuesta>("respuestas");
        }
        
        public async Task<List<Respuesta>> GetAllRespuestas()
        {
            return await _respuestas.Find(respuesta => true).ToListAsync();
        }

        public async Task<Respuesta> GetIdRespuesta(string id)
        {
            return await _respuestas.Find(respuesta => respuesta.Id == id).FirstOrDefaultAsync();
        }

        public async Task CrearRespuesta(Respuesta respuesta)
        {
            await _respuestas.InsertOneAsync(respuesta);
        }

    }
}