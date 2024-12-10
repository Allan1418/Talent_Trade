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

        public PublicacionController(UserManager<Usuario> userManager, CreadorServices creadorServices, GridFSService gridFSService, PublicacionServices publicacionServices)
        {
            _userManager = userManager;
            _creadorServices = creadorServices;
            _gridFSService = gridFSService;
            _publicacionServices = publicacionServices;
        }
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("Publicacion/{id}")]
        public IActionResult Index(string id)
        {

            var publicacion = _publicacionServices.Get(id);

            if (publicacion == null)
            {
                ModelState.AddModelError(string.Empty, "No se encontró la publicacion.");
                return View();
            }

            var modelo = new
            {
                Publicacion = publicacion
            };
            return View(modelo);
        }




    }
}
