using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class ComentarioServices
    {
        private readonly IMongoCollection<Comentario> _comentarios;

        private readonly UserManager<Usuario> _userManager;

        private readonly RespuestaServices _respuestaServices;

        public ComentarioServices(IConfiguration config, UserManager<Usuario> userManager, RespuestaServices respuestaServices)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _comentarios = database.GetCollection<Comentario>("comentarios");

            _userManager = userManager;
            _respuestaServices = respuestaServices;
        }

        public List<Comentario> GetAll()
        {
            return _comentarios.Find(comentarios => true).ToList();
        }

        public Comentario Get(string id)
        {
            return _comentarios.Find<Comentario>(comentarios => comentarios.Id == id).FirstOrDefault();
        }

        public Comentario Create(Comentario comentarios)
        {
            _comentarios.InsertOne(comentarios);
            return comentarios;
        }

        public void Update(string id, Comentario comentarioIn) =>
            _comentarios.ReplaceOne(comentario => comentario.Id == id, comentarioIn);

        public void Remove(Comentario comentarioIn) =>
            _comentarios.DeleteOne(comentario => comentario.Id == comentarioIn.Id);

        public void Remove(string id) =>
            _comentarios.DeleteOne(comentario => comentario.Id == id);

        public async Task<List<Comentario>> GetByIdPublicacionOrderFechaAsync(string idPublicacion)
        {
            var filtro = Builders<Comentario>.Filter.Eq(c => c.IdPublicacion, idPublicacion);
            var orden = Builders<Comentario>.Sort.Descending(c => c.Fecha);

            var comentarios = _comentarios.Find(filtro).Sort(orden).ToList();

            foreach (var item in comentarios)
            {
                var usuario = await _userManager.FindByIdAsync(item.IdUser);
                if (usuario != null)
                {
                    item.UserName = usuario.UserName;
                    item.FotoPerfil = usuario.ImagePerfil;
                }

                item.Respuestas = await _respuestaServices.GetByIdComentarionOrderFechaAsync(item.Id);
            }
            

            return comentarios;
        }
    }
}