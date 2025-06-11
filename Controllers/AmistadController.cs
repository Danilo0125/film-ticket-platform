using ASP_MVC_Prueba.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_MVC_Prueba.Controllers
{
    [Authorize]
    public class AmistadController : Controller
    {
        private readonly IAmistadService _amistadService;
        private readonly IUsuariosService _usuariosService;

        public AmistadController(IAmistadService amistadService, IUsuariosService usuariosService)
        {
            _amistadService = amistadService;
            _usuariosService = usuariosService;
        }

        private int ObtenerUsuarioActualId()
        {
            // Buscamos primero en los claims de autenticación
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return 0;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = ObtenerUsuarioActualId();
            if (usuarioId == 0)
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction("Index", "Home");
            }

            var amigos = await _amistadService.ObtenerAmigosAsync(usuarioId);
            var solicitudesPendientes = await _amistadService.ObtenerSolicitudesPendientesAsync(usuarioId);
            
            ViewBag.SolicitudesPendientes = solicitudesPendientes;
            ViewBag.UsuarioId = usuarioId;
            return View(amigos);
        }

        public async Task<IActionResult> SolicitudesPendientes()
        {
            var usuarioId = ObtenerUsuarioActualId();
            if (usuarioId == 0)
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction("Index", "Home");
            }

            var solicitudes = await _amistadService.ObtenerSolicitudesPendientesAsync(usuarioId);
            ViewBag.UsuarioId = usuarioId;
            return View(solicitudes);
        }

        public async Task<IActionResult> BuscarAmigos(string termino = "")
        {
            var usuarioId = ObtenerUsuarioActualId();
            if (usuarioId == 0)
            {
                TempData["Error"] = "No se pudo identificar al usuario actual.";
                return RedirectToAction("Index", "Home");
            }

            var usuarios = string.IsNullOrEmpty(termino) 
                ? new List<ASP_MVC_Prueba.Models.Usuarios>()
                : await _amistadService.BuscarUsuariosParaAgregarAsync(usuarioId, termino);
            
            ViewBag.Termino = termino;
            ViewBag.UsuarioId = usuarioId;
            return View(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarSolicitud(int usuarioReceptorId)
        {
            var usuarioId = ObtenerUsuarioActualId();
            var resultado = await _amistadService.EnviarSolicitudAmistadAsync(usuarioId, usuarioReceptorId);
            
            if (resultado)
            {
                TempData["Success"] = "Solicitud de amistad enviada correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo enviar la solicitud de amistad.";
            }
            
            return RedirectToAction("BuscarAmigos");
        }

        [HttpPost]
        public async Task<IActionResult> AceptarSolicitud(int amistadId)
        {
            var resultado = await _amistadService.AceptarSolicitudAmistadAsync(amistadId);
            
            if (resultado)
            {
                TempData["Success"] = "Solicitud de amistad aceptada.";
            }
            else
            {
                TempData["Error"] = "No se pudo aceptar la solicitud.";
            }
            
            return RedirectToAction("SolicitudesPendientes");
        }

        [HttpPost]
        public async Task<IActionResult> RechazarSolicitud(int amistadId)
        {
            var resultado = await _amistadService.RechazarSolicitudAmistadAsync(amistadId);
            
            if (resultado)
            {
                TempData["Success"] = "Solicitud de amistad rechazada.";
            }
            else
            {
                TempData["Error"] = "No se pudo rechazar la solicitud.";
            }
            
            return RedirectToAction("SolicitudesPendientes");
        }

        [HttpPost]
        public async Task<IActionResult> EliminarAmistad(int amigoId)
        {
            var usuarioId = ObtenerUsuarioActualId();
            var resultado = await _amistadService.EliminarAmistadAsync(usuarioId, amigoId);
            
            if (resultado)
            {
                TempData["Success"] = "Amistad eliminada correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar la amistad.";
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> BloquearUsuario(int usuarioABloquearId)
        {
            var usuarioId = ObtenerUsuarioActualId();
            var resultado = await _amistadService.BloquearUsuarioAsync(usuarioId, usuarioABloquearId);
            
            if (resultado)
            {
                TempData["Success"] = "Usuario bloqueado correctamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo bloquear al usuario.";
            }
            
            return RedirectToAction("Index");
        }

        // API endpoint para obtener el conteo de solicitudes pendientes
        [HttpGet]
        public async Task<JsonResult> ConteoSolicitudesPendientes()
        {
            var usuarioId = ObtenerUsuarioActualId();
            var solicitudes = await _amistadService.ObtenerSolicitudesPendientesAsync(usuarioId);
            return Json(new { count = solicitudes.Count });
        }

        // Método auxiliar para depuración
        [HttpGet]
        public IActionResult VerificarUsuario()
        {
            var usuarioId = ObtenerUsuarioActualId();
            var username = User.Identity.Name;
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            
            return Json(new { 
                UsuarioId = usuarioId, 
                Username = username,
                Claims = claims,
                IsAuthenticated = User.Identity.IsAuthenticated
            });
        }
    }
}
