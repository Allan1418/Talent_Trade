using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using Talent_Trade.Models;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Talent_Trade.Utilities;
using Talent_Trade.Services;

public class GridFSService
{
    private readonly IMongoDatabase _database;
    private readonly GridFSBucket _gridFS;

    private readonly SuscripcionServices _suscripcionServices;

    public GridFSService(IConfiguration configuration, SuscripcionServices suscripcionServices)
    {
        var connectionString = configuration.GetConnectionString("MongoDB");
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("Talent_Hub");
        _gridFS = new GridFSBucket(_database);

        _suscripcionServices = suscripcionServices;
    }

    public async Task<string> SubirImagen(IFormFile archivo, string? idNivelSuscripcion)
    {
        using (var stream = archivo.OpenReadStream())
        {
            var metadata = new BsonDocument
            {
                { "filename", archivo.FileName },
                { "contentType", archivo.ContentType },
                { "idNivelSuscripcion", idNivelSuscripcion == null ? BsonNull.Value : idNivelSuscripcion }
            };
            var id = await _gridFS.UploadFromStreamAsync(archivo.FileName, stream, new GridFSUploadOptions { Metadata = metadata });
            return id.ToString();
        }
    }

    public async Task UpdateIdNivelSuscripcionAsync(string idImagen, string? idNivelSuscripcion)
    {
        var objectId = new ObjectId(idImagen);
        var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", objectId);

        var update = Builders<GridFSFileInfo>.Update.Set( 
            x => x.Metadata["idNivelSuscripcion"], 
            idNivelSuscripcion == null ? BsonNull.Value : idNivelSuscripcion
            );

        var filesCollection = _database.GetCollection<GridFSFileInfo>(_gridFS.Options.BucketName + ".files");
        await filesCollection.UpdateOneAsync(filter, update);
    }

    public async Task EliminarImagen(string id)
    {
        var objectId = new ObjectId(id);
        await _gridFS.DeleteAsync(objectId);
    }

    public async Task<IFormFile> ObtenerImagen(string id)
    {

        try
        {

            var objectId = new ObjectId(id);
            var file = _gridFS.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", objectId)).Result.FirstOrDefault();
            var filename = file.Filename;
            var contentType = file.Metadata["contentType"].AsString;
            var imagenBytes = await _gridFS.DownloadAsBytesAsync(objectId);

            var memoryStream = new MemoryStream();
            memoryStream.Write(imagenBytes, 0, imagenBytes.Length);
            memoryStream.Position = 0;


            if (file.Metadata.Contains("idNivelSuscripcion") && !file.Metadata["idNivelSuscripcion"].IsBsonNull)
            {
                var idNivelSuscripcion = file.Metadata["idNivelSuscripcion"].AsString;

                if (!await _suscripcionServices.CheckTierAsync(idNivelSuscripcion))
                {
                    AplicarBlur(memoryStream);
                }

            }


            return new FormFile(memoryStream, 0, memoryStream.Length, filename, filename)
            {
                Headers = new HeaderDictionary { { "Content-Type", contentType } }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener la imagen: {ex.Message}");
            return null;
        }
    }

    public static void AplicarBlur(MemoryStream ms)
    {
        if (ms == null || ms.Length == 0)
        {
            return;
        }

        ms.Position = 0;

        using (var image = Image.Load(ms))
        {
            image.Mutate(x => x.BoxBlur(60));

            ms.SetLength(0);
            ms.Position = 0;

            IImageEncoder encoder = GetEncoder(image.Metadata.DecodedImageFormat.DefaultMimeType);
            image.Save(ms, encoder);
        }
    }

    private static IImageEncoder GetEncoder(string contentType)
    {
        switch (contentType)
        {
            case "image/jpeg":
                return new JpegEncoder();
            case "image/png":
                return new PngEncoder();
            default:
                return new JpegEncoder();
        }
    }
}
