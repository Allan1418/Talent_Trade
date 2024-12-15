using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharpCompress.Readers;
using System.Security.Claims;
using Talent_Trade.Models;
using Talent_Trade.Services;

namespace Talent_Trade.Controllers
{
    public class PublicacionController : Controller
    {
        private readonly UserManager<Usuario> _userManager;

        private readonly CreadorServices _creadorServices;

        private readonly GridFSService _gridFSService;

        private readonly PublicacionServices _publicacionService;

        private readonly NivelSuscripcionServices _nivelSuscripcionService;

        private readonly SuscripcionServices _suscripcionService;

        private readonly ComentarioServices _comentarioService;

        private readonly RespuestaServices _respuestaService;

        public PublicacionController(UserManager<Usuario> userManager, 
            CreadorServices creadorServices, 
            GridFSService gridFSService, 
            PublicacionServices publicacionServices, 
            NivelSuscripcionServices nivelSuscripcionServices, 
            SuscripcionServices suscripcionServices,
            ComentarioServices comentarioServices,
            RespuestaServices respuestaServices
            )
        {
            _userManager = userManager;
            _creadorServices = creadorServices;
            _gridFSService = gridFSService;
            _publicacionService = publicacionServices;
            _nivelSuscripcionService = nivelSuscripcionServices;
            _suscripcionService = suscripcionServices;
            _comentarioService = comentarioServices;
            _respuestaService = respuestaServices;

        }

        private bool EsPropietarioDePublicacion(Publicacion publicacion)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return false;
            }

            var creador = _creadorServices.GetByIdUser(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return creador != null && creador.Id == publicacion.IdCreador;
        }



        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Publicacion/{id}")]
        public async Task<ActionResult> Index(string id)
        {
            Publicacion? publicacion = null;
            List<NivelSuscripcion>? niveles = null;

            try
            {
                publicacion = _publicacionService.Get(id);
            }
            catch (Exception ex)
            {

                //ModelState.AddModelError(string.Empty, "No se encontró la publicacion.");
                //return View();
                return NotFound();
            }

            if (publicacion == null)
            {
                //ModelState.AddModelError(string.Empty, "No se encontró la publicacion.");
                //return View();
                return NotFound();
            }

            niveles = _nivelSuscripcionService.GetByCreadorIdOrderByPrecio(publicacion.IdCreador);

            publicacion.TieneAcceso = await _suscripcionService.CheckTierAsync(publicacion.IdNivelSuscripcion);

            if (publicacion.TieneAcceso)
            {
                publicacion.Comentarios = await _comentarioService.GetByIdPublicacionOrderFechaAsync(publicacion.Id);
            }
            else
            {
                publicacion.TruncarContenido(45);
            }

            var modelo = new
            {
                Publicacion = publicacion,
                EsPropietario = EsPropietarioDePublicacion(publicacion),
                Niveles = niveles,
            };

            //Console.WriteLine(modelo.EsPropietario);
            return View(modelo);
        }


        [HttpPost("EditarPublicacion")]
        public async Task<ActionResult> EditarPublicacion(string id, string titulo, string contenido, string? idNivelSuscripcion)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var publicacion = _publicacionService.Get(id);

                    if (publicacion == null)
                    {
                        return NotFound();
                    }

                    if (!EsPropietarioDePublicacion(publicacion))
                    {
                        return Unauthorized();
                    }

                    publicacion.Titulo = titulo;
                    publicacion.Contenido = contenido;

                    if (publicacion.IdNivelSuscripcion != idNivelSuscripcion)
                    {
                        foreach (var item in publicacion.Adjuntos)
                        {
                            await _gridFSService.UpdateIdNivelSuscripcionAsync(item, idNivelSuscripcion);
                        }
                    }


                    publicacion.IdNivelSuscripcion = idNivelSuscripcion;

                    _publicacionService.Update(id, publicacion);

                    return RedirectToAction("Index", "Publicacion", new { id = publicacion.Id });
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }

        [HttpPost("AgregarImagenesPublicacion")]
        public async Task<IActionResult> AgregarImagenesPublicacion(string id, List<IFormFile> imagenes)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var publicacion = _publicacionService.Get(id);

                    if (publicacion == null)
                    {
                        return NotFound();
                    }

                    if (!EsPropietarioDePublicacion(publicacion))
                    {
                        return Unauthorized();
                    }

                    foreach (var imagen in imagenes)
                    {
                        var imagenId = await _gridFSService.SubirImagen(imagen, publicacion.IdNivelSuscripcion);
                        publicacion.Adjuntos.Add(imagenId);
                    }
                    _publicacionService.Update(id, publicacion);

                    return RedirectToAction("Index", "Publicacion", new { id = publicacion.Id });
                }
                catch (Exception ex)
                {
                    return BadRequest(); ;
                }
            }
            return BadRequest();
        }


        [HttpPost("EliminarImagenPublicacion")]
        public async Task<IActionResult> EliminarImagenPublicacion(string idPublicacion, string idImagen)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var publicacion = _publicacionService.Get(idPublicacion);

                    if (publicacion == null)
                    {
                        return NotFound();
                    }

                    if (!EsPropietarioDePublicacion(publicacion))
                    {
                        return Unauthorized();
                    }

                    if (publicacion.Adjuntos == null || !publicacion.Adjuntos.Contains(idImagen))
                    {
                        return BadRequest("La imagen no pertenece a la publicacion.");
                    }

                    await _gridFSService.EliminarImagen(idImagen);

                    publicacion.Adjuntos.Remove(idImagen);

                    _publicacionService.Update(idPublicacion, publicacion);

                    return RedirectToAction("Index", "Publicacion", new { id = publicacion.Id });
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }

            return BadRequest();
        }

        [HttpPost("EliminarPublicacion")]
        public async Task<IActionResult> EliminarPublicacion(string id)
        {
            try
            {
                var publicacion = _publicacionService.Get(id);

                if (publicacion == null)
                {
                    return NotFound();
                }

                if (!EsPropietarioDePublicacion(publicacion))
                {
                    return Unauthorized();
                }

                var username = (await _userManager.GetUserAsync(User)).UserName;


                if (publicacion.Adjuntos != null)
                {
                    foreach (var idImagen in publicacion.Adjuntos)
                    {
                        await _gridFSService.EliminarImagen(idImagen);
                    }
                }


                _publicacionService.Remove(id);


                return RedirectToAction("Index", "Creador", new { username = username });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("CrearComentario")]
        public IActionResult CrearComentario(string idPublicacion, string texto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (string.IsNullOrEmpty(idUser))
                    {
                        return RedirectToAction("Login", "Home");
                    }

                    var comentario = new Comentario
                    {
                        IdPublicacion = idPublicacion,
                        Texto = texto,
                        Fecha = DateTime.Now,
                        IdUser = idUser
                    };

                    _comentarioService.Create(comentario);

                    return RedirectToAction("Index", "Publicacion", new { id = idPublicacion });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

        [HttpPost("CrearRespuesta")]
        public IActionResult CrearRespuesta(string idPublicacion, string idComentario, string texto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (string.IsNullOrEmpty(idUser))
                    {
                        return RedirectToAction("Login", "Home");
                    }

                    var respuesta = new Respuesta
                    {
                        IdComentario = idComentario,
                        Texto = texto,
                        Fecha = DateTime.Now,
                        IdUser = idUser
                    };

                    _respuestaService.Create(respuesta);

                    return RedirectToAction("Index", "Publicacion", new { id = idPublicacion });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest();
        }

    }
}
