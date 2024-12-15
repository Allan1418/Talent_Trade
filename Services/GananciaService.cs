using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class GananciaService
    {
        private readonly IMongoCollection<Ganancia> _ganancias;

        private readonly MesGananciaService _mesGananciaService;

        public GananciaService(IConfiguration config, MesGananciaService mesGananciaService)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _ganancias = database.GetCollection<Ganancia>("ganancias");

            _mesGananciaService = mesGananciaService;
        }

        public List<Ganancia> Get() =>
            _ganancias.Find(ganancia => true).ToList();

        public Ganancia Get(string id) =>
            _ganancias.Find<Ganancia>(ganancia => ganancia.Id == id).FirstOrDefault();

        public Ganancia Create(Ganancia ganancia)
        {
            _ganancias.InsertOne(ganancia);
            return ganancia;
        }

        public void Update(string id, Ganancia gananciaIn) =>
            _ganancias.ReplaceOne(ganancia => ganancia.Id == id, gananciaIn);

        public void Remove(Ganancia gananciaIn) =>
            _ganancias.DeleteOne(ganancia => ganancia.Id == gananciaIn.Id);

        public void Remove(string id) =>
            _ganancias.DeleteOne(ganancia => ganancia.Id == id);

        public Ganancia GetByIdCreador(string idCreador) =>
            _ganancias.Find<Ganancia>(ganancia => ganancia.IdCreador == idCreador).FirstOrDefault();

        public void SumarGananciaSinRetirar(string idCreador, decimal cantidad)
        {
            var filtro = Builders<Ganancia>.Filter.Eq(g => g.IdCreador, idCreador);

            var update = Builders<Ganancia>.Update
                .Inc(g => g.SinRetirar, cantidad)
                .Inc(g => g.Total, cantidad);
            _ganancias.UpdateOne(filtro, update);
            _mesGananciaService.SumarGanancia(idCreador, cantidad);
        }

        public void RetirarGanancia(string idCreador)
        {
            var filtro = Builders<Ganancia>.Filter.Eq(g => g.IdCreador, idCreador);
            var ganancia = _ganancias.Find(filtro).FirstOrDefault();

            if (ganancia == null)
            {
                throw new Exception($"No se encontro ninguna ganancia con idCreador: {idCreador}");
            }

            var update = Builders<Ganancia>.Update
                .Set(g => g.Retirado, ganancia.Retirado + ganancia.SinRetirar)
                .Set(g => g.SinRetirar, 0);
            _ganancias.UpdateOne(filtro, update);
        }

    }
}
