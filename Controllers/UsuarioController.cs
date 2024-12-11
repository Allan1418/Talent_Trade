using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Talent_Trade.Models;
using Talent_Trade.Services;

namespace Talent_Trade.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UserManager<Usuario> _userManager;

        private readonly GridFSService _gridFSService;

        public UsuarioController(UserManager<Usuario> userManager, GridFSService gridFSService)
        {
            _userManager = userManager;
            _gridFSService = gridFSService;

        }


        public async Task<IActionResult> Index()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var esCreador = false;

            var usuario = await _userManager.GetUserAsync(User);

            if (await _userManager.IsInRoleAsync(usuario, "creador"))
            {
                esCreador = true;
            }

            var modelo = new
            {
                EsCreador = esCreador,
                Usuario = usuario
            };

            return View(modelo);
        }


        [HttpPost("CambiarImagenPerfil")]
        public async Task<IActionResult> CambiarImagenPerfil(IFormFile nuevaImagen)
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

                    if (usuario.ImagePerfil != null)
                    {
                        await _gridFSService.EliminarImagen(usuario.ImagePerfil);
                    }

                    var nuevoIdImagen = await _gridFSService.SubirImagen(nuevaImagen);
                    usuario.ImagePerfil = nuevoIdImagen;

                    var result = await _userManager.UpdateAsync(usuario);

                    return RedirectToAction("Index", "Usuario");

                }
                catch (Exception ex)
                {
                    return StatusCode(500);
                }
            }
            return BadRequest();
        }
    }
}
