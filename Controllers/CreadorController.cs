using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharpCompress.Readers;
using System.Security.Claims;
using Talent_Trade.Models;
using Talent_Trade.Services;

namespace Talent_Trade.Controllers
{
    public class CreadorController : Controller
    {
        private readonly UserManager<Usuario> _userManager;

        private readonly CreadorServices _creadorServices;

        private readonly GridFSService _gridFSService;

        private readonly PublicacionServices _publicacionServices;

        private readonly NivelSuscripcionServices _nivelSuscripcionServices;

        private readonly SuscripcionServices _suscripcionServices;

        private readonly FacturaServices _facturaServices;

        public CreadorController(UserManager<Usuario> userManager, CreadorServices creadorServices,
            GridFSService gridFSService, PublicacionServices publicacionServices,
            NivelSuscripcionServices nivelSuscripcionServices, SuscripcionServices suscripcionServices,
            FacturaServices facturaServices
            )
        {
            _userManager = userManager;
            _creadorServices = creadorServices;
            _gridFSService = gridFSService;
            _publicacionServices = publicacionServices;
            _nivelSuscripcionServices = nivelSuscripcionServices;
            _suscripcionServices = suscripcionServices;
            _facturaServices = facturaServices;
        }



        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }


        [HttpGet("Creador/{username}")]
        public async Task<ActionResult> Index(string username, int pagina = 1)
        {
            var user = await _userManager.FindByNameAsync(username);

            Creador? creador = null;
            bool esPropietario = false;
            List<Publicacion>? publicaciones = null;
            string? imagenPerfil = null;
            List<NivelSuscripcion>? niveles = null;
            NivelSuscripcion? suscripcionActual = null;

            if (user != null && user.IdDeCreador != null)
            {
                creador = _creadorServices.Get(user.IdDeCreador);

                if (creador == null)
                {
                    ModelState.AddModelError(string.Empty, "No se encontró el creador.");
                    return View();
                }


                if (User.Identity.IsAuthenticated && User.FindFirstValue(ClaimTypes.NameIdentifier) == user.Id.ToString())
                {
                    esPropietario = true;
                }

                imagenPerfil = user.ImagePerfil;

                niveles = _nivelSuscripcionServices.GetByCreadorIdOrderByPrecio(creador.Id);

                publicaciones = _publicacionServices.GetPublicacionesPorCreador(creador.Id, pagina, 30);

                foreach (var item in publicaciones)
                {
                    item.TruncarContenido(45);
                    item.TieneAcceso = await _suscripcionServices.CheckTierAsync(item.IdNivelSuscripcion);
                }


                suscripcionActual = await _suscripcionServices.GetNivelSuscripcionUsuarioAsync();


            }
            else
            {
                ModelState.AddModelError(string.Empty, "No se encontró el Creador.");
                return View();
            }

            var modelo = new
            {
                Creador = creador,
                EsPropietario = esPropietario,
                Publicaciones = publicaciones,
                ImagenPerfil = imagenPerfil,
                Niveles = niveles,
                SuscripcionActual = suscripcionActual
            };
            ViewBag.Creador = creador;

            return View(modelo);
        }



        [HttpPost("EditarCreador")]
        public async Task<IActionResult> EditarCreador(string nombrePagina, string shortDescripcion, string acercaDe, IFormFile? nuevaImagen)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var usuario = await _userManager.GetUserAsync(User);

                    if (usuario == null && usuario.IdDeCreador == null)
                    {
                        return Unauthorized();
                    }

                    var creador = _creadorServices.Get(usuario.IdDeCreador);

                    if (creador == null)
                    {
                        return NotFound();
                    }

                    creador.nombrePagina = nombrePagina;
                    creador.ShortDescripcion = shortDescripcion;
                    creador.AcercaDe = acercaDe;

                    if (nuevaImagen != null)
                    {
                        if (creador.ImageBackground != null)
                        {
                            await _gridFSService.EliminarImagen(creador.ImageBackground);
                        }

                        var nuevoIdImagen = await _gridFSService.SubirImagen(nuevaImagen, null);
                        creador.ImageBackground = nuevoIdImagen;
                    }

                    _creadorServices.Update(creador.Id, creador);

                    return RedirectToAction("Index", new { username = creador.UserName });
                }
                catch (Exception ex)
                {
                    return StatusCode(500);
                }
            }


            return BadRequest();
        }


        [HttpPost("CrearPublicacion")]
        public async Task<IActionResult> CrearPublicacion(string titulo, string contenido, string? idNivelSuscripcion, List<IFormFile> archivos)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = await _userManager.GetUserAsync(User);

                    if (usuario == null)
                    {
                        return Unauthorized();
                    }

                    var publicacion = new Publicacion
                    {
                        IdCreador = usuario.IdDeCreador,
                        Titulo = titulo,
                        Contenido = contenido,
                        Fecha = DateTime.Now,
                        Likes = new List<string>(),
                        Comentarios = new List<string>(),
                        Adjuntos = new List<string>(),
                        IdNivelSuscripcion = idNivelSuscripcion
                    };

                    foreach (var archivo in archivos)
                    {
                        var archivoId = await _gridFSService.SubirImagen(archivo, idNivelSuscripcion);
                        publicacion.Adjuntos.Add(archivoId);
                    }

                    _publicacionServices.Create(publicacion);

                    return RedirectToAction("Index", "Creador", new { username = usuario.UserName });
                }
                catch (Exception ex)
                {
                    return StatusCode(500);
                }
            }
            return BadRequest();
        }

        [HttpPost("CrearNivelSuscripcion")]
        public async Task<IActionResult> CrearNivelSuscripcion(string nombre, string descripcion, decimal precio)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var usuario = await _userManager.GetUserAsync(User);

                    if (usuario == null && !await _userManager.IsInRoleAsync(usuario, "creador"))
                    {
                        return Unauthorized();
                    }

                    var nivelSuscripcion = new NivelSuscripcion
                    {
                        IdCreador = usuario.IdDeCreador,
                        Nombre = nombre,
                        Descripcion = descripcion,
                        Precio = precio
                    };

                    _nivelSuscripcionServices.Create(nivelSuscripcion);

                    return RedirectToAction("Index", "Creador", new { username = usuario.UserName });
                }
                catch (Exception ex)
                {
                    return StatusCode(500);
                }
            }

            return BadRequest();
        }

        [HttpPost("EditarNivelSuscripcion")]
        public async Task<IActionResult> EditarNivelSuscripcion(string id, string nombre, string descripcion, decimal precio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuario = await _userManager.GetUserAsync(User);

                    if (usuario == null)
                    {
                        return Unauthorized();
                    }

                    var nivelSuscripcion = _nivelSuscripcionServices.Get(id);

                    if (nivelSuscripcion == null)
                    {
                        return NotFound();
                    }

                    if (nivelSuscripcion.IdCreador != usuario.IdDeCreador)
                    {
                        return Forbid();
                    }

                    nivelSuscripcion.Nombre = nombre;
                    nivelSuscripcion.Descripcion = descripcion;
                    nivelSuscripcion.Precio = precio;

                    _nivelSuscripcionServices.Update(id, nivelSuscripcion);

                    return RedirectToAction("Index", "Creador", new { username = usuario.UserName });
                }
                catch (Exception ex)
                {
                    return StatusCode(500);
                }
            }

            return BadRequest();
        }


        [HttpPost("EliminarNivelSuscripcion")]
        public async Task<IActionResult> EliminarNivelSuscripcion(string id)
        {
            try
            {
                var usuario = await _userManager.GetUserAsync(User);

                if (usuario == null)
                {
                    return Unauthorized();
                }

                var nivelSuscripcion = _nivelSuscripcionServices.Get(id);

                if (nivelSuscripcion == null)
                {
                    return NotFound();
                }

                if (nivelSuscripcion.IdCreador != usuario.IdDeCreador)
                {
                    return Forbid();
                }

                _nivelSuscripcionServices.Remove(id);

                return RedirectToAction("Index", "Creador", new { username = usuario.UserName });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost("Suscribirse/{idNivelSuscripcion}")]
        public async Task<IActionResult> Suscribirse(string idNivelSuscripcion)
        {

            Suscripcion? suscripcionVieja = null;

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                return RedirectToAction("Login", "Home");
            }


            var nivelSuscripcion = _nivelSuscripcionServices.Get(idNivelSuscripcion);
            if (nivelSuscripcion == null)
            {
                return NotFound("Nivel de suscripción no encontrado.");
            }

            var creador = _creadorServices.Get(nivelSuscripcion.IdCreador);
            if (creador == null)
            {
                return StatusCode(500);
            }

            if (!await _suscripcionServices.CheckTierAsync(nivelSuscripcion.Id))
            {

                suscripcionVieja = await _suscripcionServices.GetSuscripcionByCreadorAsync(nivelSuscripcion.IdCreador);

                if (suscripcionVieja != null)
                {
                    _suscripcionServices.Remove(suscripcionVieja.Id);
                }

                var nuevaSuscripcion = new Suscripcion
                {
                    IdUser = usuario.Id.ToString(),
                    IdNivelSuscripcion = nivelSuscripcion.Id,
                    IdCreador = nivelSuscripcion.IdCreador,
                    FechaVencimiento = DateTime.Now.AddMonths(1)
                };

                nuevaSuscripcion = _suscripcionServices.Create(nuevaSuscripcion);

                var nuevaFactura = new Factura
                {
                    IdUser = usuario.Id.ToString(),
                    IdCreador = creador.Id,
                    Monto = nivelSuscripcion.Precio,
                    FechaPago = DateTime.Now
                };

                _facturaServices.Create(nuevaFactura);



            }
            else
            {
                ModelState.AddModelError(string.Empty, "Ya tienes una suscripción a un nivel superior para este creador.");
                return RedirectToAction("Index", "Creador", new { username = creador.UserName });
            }

            return RedirectToAction("Index", "Creador", new { username = creador.UserName });
        }


    }
}
