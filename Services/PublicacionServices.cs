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
        public List<Publicacion> GetAll() =>
            _publicaciones.Find(publicacion => true).ToList();

        public Publicacion Get(string id) =>
            _publicaciones.Find<Publicacion>(publicacion => publicacion.Id == id).FirstOrDefault();

        public Publicacion Create(Publicacion publicacion)
        {
            _publicaciones.InsertOne(publicacion);
            return publicacion;
        }

        public void Update(string id, Publicacion publicacionIn) =>
            _publicaciones.ReplaceOne(publicacion => publicacion.Id == id, publicacionIn);

        public void Remove(Publicacion publicacionIn) =>
            _publicaciones.DeleteOne(publicacion => publicacion.Id == publicacionIn.Id);

        public void Remove(string id) =>
            _publicaciones.DeleteOne(publicacion => publicacion.Id == id);
    }
}