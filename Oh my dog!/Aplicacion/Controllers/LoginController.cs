using Aplicacion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Diagnostics.Contracts;

namespace Aplicacion.Controllers
{
    public class LoginController : Controller
    {
        private readonly OhmydogdbContext _context;
        public LoginController(OhmydogdbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
			if (User.Identity!.IsAuthenticated)
            {
				return RedirectToAction("Index", "Home");
			}
			return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Usuarios _usuario)
        {
            Usuarios? usuario = null;
            usuario = _context.Usuarios.Where(u => (u.Email == _usuario.Email) && (u.Pass == _usuario.Pass)).FirstOrDefault();
            if (usuario != null)
            {
                if (usuario.Estado == 0)
                {
                    ViewBag.Message = "El usuario está baneado!. Por favor visite la veterinaria o póngase en contacto a través de ohmydog@gmail.com";
                    return View();
                }

                Rol? rolUser = new Rol();
                rolUser = _context.Rols.Where(r => r.IdRol == usuario!.IdRol).FirstOrDefault();

                // CONFIGURACION DE LA AUTENTICACION
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim("Email", usuario.Email),
                    new Claim("New", (usuario.Pass[0] == '?') ? "true" : "false")
                };

                claims.Add(new Claim(ClaimTypes.Role, rolUser!.Descripcion));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Message = "Error al iniciar sesión, el email o contraseña son incorrectos!";
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<JsonResult> CambioDeContrasena()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json(true);
        }

        public Usuarios? validarUsuario(string email, string password)
        {
            Usuarios? user = new Usuarios();
            user = _context.Usuarios.Where(u => u.Email == email && u.Pass == password).FirstOrDefault();
            return (user);

        }

    }
}
