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

        public CreadorController(UserManager<Usuario> userManager, CreadorServices creadorServices, GridFSService gridFSService, PublicacionServices publicacionServices)
        {
            _userManager = userManager;
            _creadorServices = creadorServices;
            _gridFSService = gridFSService;
            _publicacionServices = publicacionServices;
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
                ImagenPerfil = imagenPerfil
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
                    ModelState.AddModelError(string.Empty, $"Error al editar el creador: {ex.Message}");
                }
            }

            
            return RedirectToAction("Index", "Home");
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
                    ModelState.AddModelError(string.Empty, $"Error al crear la publicación: {ex.Message}");
                }
            }
            Console.WriteLine("---errorrr");
            return RedirectToAction("Index", "Home");
        }





        // GET: CreadorController/Details/5
        public ActionResult Detalle(int id)
        {
            return View();
        }

        // GET: CreadorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CreadorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CreadorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CreadorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CreadorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CreadorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
