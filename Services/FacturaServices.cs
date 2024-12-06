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
        
    }
}