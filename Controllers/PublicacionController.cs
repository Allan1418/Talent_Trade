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

        private readonly PublicacionServices _publicacionServices;

        private readonly NivelSuscripcionServices _nivelSuscripcionServices;

        private readonly SuscripcionServices _suscripcionServices;

        public PublicacionController(UserManager<Usuario> userManager, CreadorServices creadorServices, 
            GridFSService gridFSService, PublicacionServices publicacionServices, 
            NivelSuscripcionServices nivelSuscripcionServices, SuscripcionServices suscripcionServices
            )
        {
            _userManager = userManager;
            _creadorServices = creadorServices;
            _gridFSService = gridFSService;
            _publicacionServices = publicacionServices;
            _nivelSuscripcionServices = nivelSuscripcionServices;
            _suscripcionServices = suscripcionServices;

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
                publicacion = _publicacionServices.Get(id);
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

            niveles = _nivelSuscripcionServices.GetByCreadorIdOrderByPrecio(publicacion.IdCreador);

            publicacion.TieneAcceso = await _suscripcionServices.CheckTierAsync(publicacion.IdNivelSuscripcion);

            if (!publicacion.TieneAcceso)
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

                    var publicacion = _publicacionServices.Get(id);

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

                    _publicacionServices.Update(id, publicacion);

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
                    var publicacion = _publicacionServices.Get(id);

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
                    _publicacionServices.Update(id, publicacion);

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
                    var publicacion = _publicacionServices.Get(idPublicacion);

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

                    _publicacionServices.Update(idPublicacion, publicacion);

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
                var publicacion = _publicacionServices.Get(id);

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


                _publicacionServices.Remove(id);


                return RedirectToAction("Index", "Creador", new { username = username });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }



    }
}
