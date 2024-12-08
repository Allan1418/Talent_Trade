using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Talent_Trade.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.Identity.Mongo.Model;

namespace Talent_Trade.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly SignInManager<Usuario> _signInManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<Usuario> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Home/login")]
        public async Task<IActionResult> Login(string Username, string Contrasena)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine($"DatosUsuario:");
                Console.WriteLine($"User---" + Username + "---");
                Console.WriteLine($"Contra---" + Contrasena + "---");

                var userManager = HttpContext.RequestServices.GetService<UserManager<Usuario>>();
                var user = await userManager.FindByEmailAsync(Username);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, Contrasena, false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        // Login exitoso
                        Console.WriteLine($"Login exitoso!!!");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Login fallido
                        Console.WriteLine($"Login fallido!!!");
                        ModelState.AddModelError(string.Empty, "Intento de inicio de sesión fallido.");
                        return View(); // O redirige a la página de login con el error
                    }
                }
                else
                {
                    // El usuario no se encontro
                    Console.WriteLine($"El usuario no se encontro!!!");
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión fallido.");
                    return View();
                }
            }
            return View(); // O redirige a la página de login con el error
        }

        public IActionResult SignUp()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
