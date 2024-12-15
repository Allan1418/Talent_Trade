using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class MesGananciaService
    {
        private readonly IMongoCollection<MesGanancia> _mesGanancias;

        public MesGananciaService(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _mesGanancias = database.GetCollection<MesGanancia>("mesGanancias");
        }

        public List<MesGanancia> Get() =>
            _mesGanancias.Find(mesGanancia => true).ToList();

        public MesGanancia Get(string id) =>
            _mesGanancias.Find<MesGanancia>(mesGanancia => mesGanancia.Id == id).FirstOrDefault();

        public MesGanancia Create(MesGanancia mesGanancia)
        {
            _mesGanancias.InsertOne(mesGanancia);
            return mesGanancia;
        }

        public void Update(string id, MesGanancia mesGananciaIn) =>
            _mesGanancias.ReplaceOne(mesGanancia => mesGanancia.Id == id, mesGananciaIn);

        public void Remove(MesGanancia mesGananciaIn) =>
            _mesGanancias.DeleteOne(mesGanancia => mesGanancia.Id == mesGananciaIn.Id);

        public void Remove(string id) =>
            _mesGanancias.DeleteOne(mesGanancia => mesGanancia.Id == id);

        public List<MesGanancia> GetByIdCreador(string idCreador) =>
            _mesGanancias.Find(mesGanancia => mesGanancia.IdCreador == idCreador).ToList();

        public void SumarGanancia(string idCreador, decimal cantidad)
        {

            int monthActual = DateTime.Now.Month;
            int yearActual = DateTime.Now.Year;

            var filtro = Builders<MesGanancia>.Filter.And(
                Builders<MesGanancia>.Filter.Eq(mg => mg.IdCreador, idCreador),
                Builders<MesGanancia>.Filter.Eq(mg => mg.Month, monthActual),
                Builders<MesGanancia>.Filter.Eq(mg => mg.Year, yearActual)
            );
            var mesGanancia = _mesGanancias.Find(filtro).FirstOrDefault();

            if (mesGanancia == null)
            {
                mesGanancia = new MesGanancia
                {
                    IdCreador = idCreador,
                    Month = monthActual,
                    Year = yearActual,
                    Total = cantidad
                };
                _mesGanancias.InsertOne(mesGanancia);
            }
            else
            {

                var update = Builders<MesGanancia>.Update.Inc(mg => mg.Total, cantidad);
                _mesGanancias.UpdateOne(filtro, update);
            }
        }
    }
}