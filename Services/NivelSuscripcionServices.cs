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
        public List<NivelSuscripcion> GetAll() =>
            _nivelesSuscripciones.Find(nivelesSuscripciones => true).ToList();

        public NivelSuscripcion Get(string id) =>
            _nivelesSuscripciones.Find<NivelSuscripcion>(nivelesSuscripciones => nivelesSuscripciones.Id == id).FirstOrDefault();

        public NivelSuscripcion Create(NivelSuscripcion nivelesSuscripciones)
        {
            _nivelesSuscripciones.InsertOne(nivelesSuscripciones);
            return nivelesSuscripciones;
        }

        public void Update(string id, NivelSuscripcion nivelSuscripcionIn) =>
            _nivelesSuscripciones.ReplaceOne(nivelSuscripcion => nivelSuscripcion.Id == id, nivelSuscripcionIn);

        public void Remove(NivelSuscripcion nivelSuscripcionIn) =>
            _nivelesSuscripciones.DeleteOne(nivelSuscripcion => nivelSuscripcion.Id == nivelSuscripcionIn.Id);

        public void Remove(string id) =>
            _nivelesSuscripciones.DeleteOne(nivelSuscripcion => nivelSuscripcion.Id == id);
    }
}