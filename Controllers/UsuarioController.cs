using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Talent_Trade.Models;
using Talent_Trade.Services;

namespace Talent_Trade.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UserManager<Usuario> _userManager;

        private readonly GridFSService _gridFSService;

        private readonly FacturaServices _facturaServices;

        private readonly GananciaService _gananciaService;

        private readonly MesGananciaService _mesGananciaService;

        private readonly CreadorServices _creadorServices;

        public UsuarioController(UserManager<Usuario> userManager, 
            GridFSService gridFSService, 
            FacturaServices facturaServices, 
            GananciaService gananciaService,
            MesGananciaService mesGananciaService,
            CreadorServices creadorServices
            )
        {
            _userManager = userManager;
            _gridFSService = gridFSService;
            _facturaServices = facturaServices;
            _gananciaService = gananciaService;
            _mesGananciaService = mesGananciaService;
            _creadorServices = creadorServices;

        }


        public async Task<IActionResult> Index()
        {

            List<Factura>? facturas = null;
            Ganancia? ganancia = null;
            List<MesGanancia>? mesesGanancias = null;

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var esCreador = false;

            var usuario = await _userManager.GetUserAsync(User);

            if (await _userManager.IsInRoleAsync(usuario, "creador"))
            {
                esCreador = true;
                ganancia = _gananciaService.GetByIdCreador(usuario.IdDeCreador);
                mesesGanancias = _mesGananciaService.GetByIdCreador(usuario.IdDeCreador);
            }

            facturas = _facturaServices.GetByIdUser(usuario.Id.ToString());

            var modelo = new
            {
                EsCreador = esCreador,
                Usuario = usuario,
                Facturas = facturas,
                Ganancia = ganancia,
                MesesGanancias = mesesGanancias
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

                    var nuevoIdImagen = await _gridFSService.SubirImagen(nuevaImagen, null);
                    usuario.ImagePerfil = nuevoIdImagen;

                    var result = await _userManager.UpdateAsync(usuario);


                    if (await _userManager.IsInRoleAsync(usuario, "creador"))
                    {
                        var creador = _creadorServices.Get(usuario.IdDeCreador);
                        if (creador != null)
                        {
                            creador.ImagePerfil = nuevoIdImagen;
                            _creadorServices.Update(creador.Id, creador);
                        }

                    }


                    return RedirectToAction("Index", "Usuario");

                }
                catch (Exception ex)
                {
                    return StatusCode(500);
                }
            }
            return BadRequest();
        }


        [HttpPost("retirarGanancia")]
        public async Task<IActionResult> RetirarGanancia()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null || !await _userManager.IsInRoleAsync(usuario, "creador"))
            {
                return Unauthorized();
            }
            try
            {
                _gananciaService.RetirarGanancia(usuario.IdDeCreador);
                return RedirectToAction("Index", "Usuario");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
