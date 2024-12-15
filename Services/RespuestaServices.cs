using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class RespuestaServices
    {
        private readonly IMongoCollection<Respuesta> _respuestas;

        private readonly UserManager<Usuario> _userManager;

        public RespuestaServices(IConfiguration config, UserManager<Usuario> userManager)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _respuestas = database.GetCollection<Respuesta>("respuestas");

            _userManager = userManager;
        }

        public List<Respuesta> GetAll() =>
            _respuestas.Find(respuestas => true).ToList();

        public Respuesta Get(string id) =>
            _respuestas.Find<Respuesta>(respuestas => respuestas.Id == id).FirstOrDefault();

        public Respuesta Create(Respuesta respuestas)
        {
            _respuestas.InsertOne(respuestas);
            return respuestas;
        }

        public void Update(string id, Respuesta respuestaIn) =>
            _respuestas.ReplaceOne(Respuesta => Respuesta.Id == id, respuestaIn);

        public void Remove(Respuesta respuestaIn) =>
            _respuestas.DeleteOne(Respuesta => Respuesta.Id == respuestaIn.Id);

        public void Remove(string id) =>
            _respuestas.DeleteOne(Respuesta => Respuesta.Id == id);

        public async Task<List<Respuesta>> GetByIdComentarionOrderFechaAsync(string idComentario)
        {
            var filtro = Builders<Respuesta>.Filter.Eq(c => c.IdComentario, idComentario);
            var orden = Builders<Respuesta>.Sort.Descending(c => c.Fecha);

            var respuestas = _respuestas.Find(filtro).Sort(orden).ToList();

            foreach (var item in respuestas)
            {
                var usuario = await _userManager.FindByIdAsync(item.IdUser);
                if (usuario != null)
                {
                    item.UserName = usuario.UserName;
                    item.FotoPerfil = usuario.ImagePerfil;
                }

            }


            return respuestas;
        }
    }
}