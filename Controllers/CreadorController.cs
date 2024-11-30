using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Talent_Trade.Controllers
{
    public class CreadorController : Controller
    {
        // GET: CreadorController
        public ActionResult Index()
        {
            return View();
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
