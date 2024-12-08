using MongoDB.Bson;
using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class UsuarioService
    {
        private readonly IMongoCollection<Usuario> _usuarios;

        public UsuarioService(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _usuarios = database.GetCollection<Usuario>("usuarios");
        }

        public List<Usuario> GetAll() =>
            _usuarios.Find(usuario => true).ToList();

        public Usuario Get(string id) =>
            _usuarios.Find<Usuario>(usuario => usuario.Id == ObjectId.Parse(id)).FirstOrDefault();

        public Usuario Create(Usuario usuario)
        {
            _usuarios.InsertOne(usuario);
            return usuario;
        }

        public void Update(ObjectId id, Usuario usuarioIn) =>
            _usuarios.ReplaceOne(usuario => usuario.Id == id, usuarioIn);

        public void Remove(Usuario usuarioIn) =>
            _usuarios.DeleteOne(usuario => usuario.Id == usuarioIn.Id);

        public void Remove(string id) =>
            _usuarios.DeleteOne(usuario => usuario.Id == ObjectId.Parse(id));
    }
}
