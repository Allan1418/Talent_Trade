using MongoDB.Driver;
using Talent_Trade.Models;

namespace Talent_Trade.Services
{
    public class FacturaServices
    {
        private readonly IMongoCollection<Factura> _facturas;

        public FacturaServices(IConfiguration config)
        {
            MongoClient client = new MongoClient(config.GetConnectionString("MongoDB"));
            IMongoDatabase database = client.GetDatabase("Talent_Hub");
            _facturas = database.GetCollection<Factura>("facturas");
        }

        public List<Factura> GetAll() =>
            _facturas.Find(facturas => true).ToList();

        public Factura Get(string id) =>
            _facturas.Find<Factura>(facturas => facturas.Id == id).FirstOrDefault();

        public Factura Create(Factura facturas)
        {
            _facturas.InsertOne(facturas);
            return facturas;
        }

        public void Update(string id, Factura facturaIn) =>
            _facturas.ReplaceOne(factura => factura.Id == id, facturaIn);

        public void Remove(Factura facturaIn) =>
            _facturas.DeleteOne(factura => factura.Id == facturaIn.Id);

        public void Remove(string id) =>
            _facturas.DeleteOne(factura => factura.Id == id);
    }
}