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

        public List<Suscripcion> GetAll() =>

            _suscripciones.Find(suscripcion => true).ToList();

        public Suscripcion Get (string id) =>

            _suscripciones.Find<Suscripcion>(suscripcion => suscripcion.Id == id).FirstOrDefault();

        public Suscripcion Create(Suscripcion suscripcion)
        {
            _suscripciones.InsertOne(suscripcion);
            return suscripcion;
        }

        public void Update(string id, Suscripcion suscripcionIn) =>
            _suscripciones.ReplaceOne(suscripcion => suscripcion.Id == id, suscripcionIn);

        public void Remove(Suscripcion suscripcionIn) =>
            _suscripciones.DeleteOne(suscripcion => suscripcion.Id == suscripcionIn.Id);

        public void Remove(string id) =>
            _suscripciones.DeleteOne(suscripcion => suscripcion.Id == id);
    }
}