using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Talent_Trade.Models;
using Talent_Trade.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Authorization;
using Talent_Trade.Services;

namespace Talent_Trade.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly SignInManager<Usuario> _signInManager;

        private readonly UserManager<Usuario> _userManager;

        private readonly CreadorServices _creadorServices;

        private readonly GridFSService _gridFSService;

        private readonly SuscripcionServices _suscripcionServices;

        private readonly GananciaService _gananciaService;

        public HomeController(
            ILogger<HomeController> logger, 
            SignInManager<Usuario> signInManager, 
            UserManager<Usuario> userManager, 
            CreadorServices creadorServices, 
            GridFSService gridFSService, 
            SuscripcionServices suscripcionServices,
            GananciaService gananciaService
            )
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _creadorServices = creadorServices;
            _gridFSService = gridFSService;
            _suscripcionServices = suscripcionServices;
            _gananciaService = gananciaService;
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

                        // check suscripciones
                        foreach (var item in await _suscripcionServices.GetSuscripcionesUsuarioAsync())
                        {
                            if (item.FechaVencimiento < DateTime.Now)
                            {
                                _suscripcionServices.Remove(item.Id);
                            }
                        }

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
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(string NombreCompleto, string Username, string Correo, string Contrasena, string ConfirmarContrasena)
        {
            if (ModelState.IsValid)
            {
                if (Contrasena != ConfirmarContrasena)
                {
                    ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden.");
                    return View();
                }

                var usuario = new Usuario
                {
                    NombreCompleto = NombreCompleto,
                    UserName = Username,
                    Email = Correo,
                    FechaRegistro = DateTime.Now
                };

                var result = await _userManager.CreateAsync(usuario, Contrasena);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(usuario, "usuario");
                    await _userManager.UpdateAsync(usuario);

                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //Volverse creador
        //[Authorize]
        public async Task<IActionResult> VolverseCreador()
        {

            var usuario = await _userManager.GetUserAsync(User);

            if (usuario != null && !await _userManager.IsInRoleAsync(usuario, "creador"))
            {
                await _userManager.AddToRoleAsync(usuario, "creador");

                await _userManager.UpdateAsync(usuario);

                await _signInManager.RefreshSignInAsync(usuario);

                var creador = new Creador
                {
                    IdUser = usuario.Id.ToString(),
                    UserName = usuario.UserName,
                    nombrePagina = "",
                    ShortDescripcion = "",
                    AcercaDe = ""
                };

                creador = _creadorServices.Create(creador);
                usuario.IdDeCreador = creador.Id;

                var ganancia = new Ganancia
                {
                    IdCreador = usuario.IdDeCreador,
                    SinRetirar = 0,
                    Retirado = 0,
                    Total = 0
                };
                _gananciaService.Create(ganancia);


                await _userManager.UpdateAsync(usuario);


            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet("File/{id}")]
        public async Task<IActionResult> File(string id)
        {
            var imagen = await _gridFSService.ObtenerImagen(id);

            if (imagen == null)
            {
                return NotFound();
            }
            return File(imagen.OpenReadStream(), imagen.ContentType);
        }
    }
}
