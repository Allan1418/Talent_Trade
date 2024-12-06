using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class CreadorServices
    {
        private readonly IMongoCollection<Creador> _creadores;

        public CreadorServices(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _creadores = database.GetCollection<Creador>("creadores");
        }

        public List<Creador> GetAllCreadores() =>
            _creadores.Find(creador => true).ToList();

        public Creador GetIdCreador(string id) =>
            _creadores.Find<Creador>(creador => creador.Id == id).FirstOrDefault();

        public Creador Create(Creador creador)
        {
            _creadores.InsertOne(creador);
            return creador;
        }

        public void Update(string id, Creador creadorIn) =>
            _creadores.ReplaceOne(creador => creador.Id == id, creadorIn);

        public void Remove(Creador creadorIn) =>
            _creadores.DeleteOne(creador => creador.Id == creadorIn.Id);

        public void Remove(string id) =>
            _creadores.DeleteOne(creador => creador.Id == id);
    }
}