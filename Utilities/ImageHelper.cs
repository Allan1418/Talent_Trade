using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Http;
using System.IO;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats;

namespace Talent_Trade.Utilities
{
    public static class ImageHelper
    {
        public static IFormFile AplicarBlur(IFormFile archivoImagen)
        {
            if (archivoImagen == null || archivoImagen.Length == 0)
            {
                return null;
            }

            IFormFile imagenProcesada = null;
            MemoryStream msCopia = null; // Para almacenar la copia del MemoryStream

            using (var image = Image.Load(archivoImagen.OpenReadStream()))
            {
                image.Mutate(x => x.BoxBlur(60));

                using (var ms = new MemoryStream())
                {
                    IImageEncoder encoder = GetEncoder(archivoImagen.ContentType);

                    image.Save(ms, encoder);
                    ms.Position = 0;

                    // Crear una copia del MemoryStream
                    msCopia = new MemoryStream(ms.ToArray());

                    imagenProcesada = new FormFile(msCopia, 0, msCopia.Length, archivoImagen.Name, archivoImagen.FileName)
                    {
                        Headers = archivoImagen.Headers,
                        ContentType = archivoImagen.ContentType
                    };
                }
            }

            return imagenProcesada; // Retornar el IFormFile con la copia del MemoryStream
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
}
