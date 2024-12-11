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

        public CreadorController(UserManager<Usuario> userManager, CreadorServices creadorServices,
            GridFSService gridFSService, PublicacionServices publicacionServices,
            NivelSuscripcionServices nivelSuscripcionServices
            )
        {
            _userManager = userManager;
            _creadorServices = creadorServices;
            _gridFSService = gridFSService;
            _publicacionServices = publicacionServices;
            _nivelSuscripcionServices = nivelSuscripcionServices;
        }




        // GET: CreadorController
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
                }

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
                Niveles = niveles
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

                        var nuevoIdImagen = await _gridFSService.SubirImagen(nuevaImagen);
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
        public async Task<IActionResult> CrearPublicacion(string titulo, string contenido, List<IFormFile> archivos)
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
                        Adjuntos = new List<string>()
                    };

                    foreach (var archivo in archivos)
                    {
                        var archivoId = await _gridFSService.SubirImagen(archivo);
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



    }
}
