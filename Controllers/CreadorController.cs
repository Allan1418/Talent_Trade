using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

                publicaciones = _publicacionServices.GetPublicacionesPorCreador(creador.Id, pagina, 30);

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
                Publicaciones = publicaciones
            };

            return View(modelo);
        }

        [HttpGet("MostrarImagen/{id}")]
        public async Task<IActionResult> MostrarImagen(string id)
        {
            var imagen = await _gridFSService.ObtenerImagen(id);
            return File(imagen.OpenReadStream(), imagen.ContentType);
        }

        [HttpPost("EditarCreador")]
        public async Task<IActionResult> EditarCreador(Creador creador, IFormFile? nuevaImagen)
        {
            if (ModelState.IsValid)
            {
                try
                {
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

            // Si hay errores de validación o una excepción, volver a mostrar la vista de edición
            return View(creador);
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
