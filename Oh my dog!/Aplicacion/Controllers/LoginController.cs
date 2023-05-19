using Aplicacion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Usuarios _usuario)
        {
            Usuarios usuario = null;
            usuario = this.validarUsuario(_usuario.Email, _usuario.Pass);
            if (usuario != null)
            {
                Rol rolUser = new Rol();
                rolUser = _context.Rols.Where(r => r.IdRol == usuario.IdRol).FirstOrDefault();
                // CONFIGURACION DE LA AUTENTICACION
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim("Email", usuario.Email),
                };

                claims.Add(new Claim(ClaimTypes.Role, rolUser.Descripcion));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public Usuarios validarUsuario(string email, string password)
        {
            Usuarios user = new Usuarios();
            user = _context.Usuarios.Where(u => u.Email == email && u.Pass == password).FirstOrDefault();
            return user;

        }

    }
}
