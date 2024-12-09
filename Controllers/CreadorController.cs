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

        public CreadorController(UserManager<Usuario> userManager, CreadorServices creadorServices)
        {
            _userManager = userManager;
            _creadorServices = creadorServices;
        }




        // GET: CreadorController
        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }


        [HttpGet("Creador/{username}")]
        public async Task<ActionResult> Index(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            Creador? creador = null;
            bool esPropietario = false;

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
            }
            else
            {
                ModelState.AddModelError(string.Empty, "No se encontró el Creador.");
                return View();
            }

            var modelo = new { Creador = creador, EsPropietario = esPropietario };

            return View(modelo);
        }

        // GET: CreadorController/Details/5
        public ActionResult Details(int id)
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
