using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

public class GridFSService
{
    private readonly IMongoDatabase _database;
    private readonly GridFSBucket _gridFS;

    public GridFSService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB");
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("Talent_Hub");
        _gridFS = new GridFSBucket(_database);
    }

    public async Task<string> SubirImagen(IFormFile archivo)
    {
        using (var stream = archivo.OpenReadStream())
        {
            var metadata = new BsonDocument
            {
                { "filename", archivo.FileName },
                { "contentType", archivo.ContentType }
            };
            var id = await _gridFS.UploadFromStreamAsync(archivo.FileName, stream, new GridFSUploadOptions { Metadata = metadata });
            return id.ToString();
        }
    }

    public async Task<IFormFile> ObtenerImagen(string id)
    {
        var objectId = new ObjectId(id);

        try
        {
            // Obtener la imagen y los metadatos
            using (var stream = await _gridFS.OpenDownloadStreamAsync(objectId))
            {
                var file = _gridFS.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", objectId)).Result.FirstOrDefault();
                var filename = file.Filename;
                var contentType = file.Metadata["contentType"].AsString;
                var imagenBytes = await _gridFS.DownloadAsBytesAsync(objectId);

                using (var memoryStream = new MemoryStream(imagenBytes))
                {
                    return new FormFile(memoryStream, 0, memoryStream.Length, filename, filename) { Headers = new HeaderDictionary { { "Content-Type", contentType } } };
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener la imagen: {ex.Message}");
            return null;
        }
    }

    public async Task EliminarImagen(string id)
    {
        var objectId = new ObjectId(id);
        await _gridFS.DeleteAsync(objectId);
    }
}
