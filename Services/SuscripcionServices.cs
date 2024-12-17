using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class SuscripcionServices
    {
        private readonly IMongoCollection<Suscripcion> _suscripciones;

        private readonly UserManager<Usuario> _userManager;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly NivelSuscripcionServices _nivelSuscripcionServices;

        public SuscripcionServices(IConfiguration config, UserManager<Usuario> userManager, IHttpContextAccessor httpContextAccessor, NivelSuscripcionServices nivelSuscripcionServices)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _suscripciones = database.GetCollection<Suscripcion>("suscripciones");

            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _nivelSuscripcionServices = nivelSuscripcionServices;

        }

        public List<Suscripcion> GetAll() =>

            _suscripciones.Find(suscripcion => true).ToList();

        public Suscripcion Get(string id) =>

            _suscripciones.Find<Suscripcion>(suscripcion => suscripcion.Id == id).FirstOrDefault();

        public async Task<Suscripcion?> GetSuscripcionByCreadorAsync(string? idCreador)
        {
            var usuario = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);

            if (usuario == null || idCreador == null)
            {
                return null;
            }

            return await _suscripciones.Find(s => s.IdUser == usuario.Id.ToString() && s.IdCreador == idCreador).FirstOrDefaultAsync();
        }

        public async Task<List<Suscripcion>> GetSuscripcionesUsuarioAsync()
        {
            var usuario = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);

            if (usuario == null)
            {
                return new List<Suscripcion>();
            }

            return await _suscripciones.Find(s => s.IdUser == usuario.Id.ToString()).ToListAsync();
        }

        public async Task<NivelSuscripcion?> GetNivelSuscripcionUsuarioAsync(string idCreador)
        {
            var usuario = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
            if (usuario == null)
            {
                return null;
            }

            var nivelSuscripcion = await _suscripciones.Aggregate()
                .Match(s => s.IdUser == usuario.Id.ToString() && s.IdCreador == idCreador)
                .Lookup("nivelesSuscripciones", "idNivelSuscripcion", "_id", "nivelSuscripcion")
                .Unwind("nivelSuscripcion")
                .ReplaceRoot<NivelSuscripcion>("$nivelSuscripcion")
                .FirstOrDefaultAsync();

            //Console.WriteLine(nivelSuscripcion.ToJson());

            return nivelSuscripcion;
        }

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

        public async Task<bool> CheckTierAsync(string? idNivelSuscripcion)
        {
            if (idNivelSuscripcion == null || idNivelSuscripcion == "")
            {
                return true;
            }

            NivelSuscripcion tier = null;
            try
            {
                tier = _nivelSuscripcionServices.Get(idNivelSuscripcion);
            }
            catch (Exception)
            {
                return true;
            }

            if (tier == null)
            {
                return true;
            }

            var usuario = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);

            if (usuario == null)
            {
                return false;
            }

            if (await _userManager.IsInRoleAsync(usuario, "creador"))
            {
                if (usuario.IdDeCreador == tier.IdCreador)
                {
                    return true;
                }
            }



            //consulta bd
            var suscripcion = await _suscripciones.Aggregate()
                .Match(s => s.IdUser == usuario.Id.ToString() && s.IdCreador == tier.IdCreador)
                .Lookup("nivelesSuscripciones", "idNivelSuscripcion", "_id", "nivel")
                .Unwind("nivel")
                .FirstOrDefaultAsync();

            if (suscripcion == null)
            {
                return false;
            }

            //Console.WriteLine(suscripcion.ToJson());

            var precio = suscripcion["nivel"]["precio"].AsDecimal;

            return precio >= tier.Precio;

        }
    }
    public class SuscripcionConNivel : Suscripcion
    {
        public NivelSuscripcion nivel { get; set; }
    }
}