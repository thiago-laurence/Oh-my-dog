using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Models;
using System.Text.RegularExpressions;
using Aplicacion.Data;
using Microsoft.Identity.Client;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Versioning;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MailKit.Security;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System.Runtime.CompilerServices;

namespace Aplicacion.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly OhmydogdbContext _context;

        public UsuariosController(OhmydogdbContext context)
        {
            _context = context;
        }

        // GET: Usuarios

        // Variable para indicar la cantida a mostrar por la paginacion
        private int cantidadDeRegistros = 3;
        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveView = "IndexClientes";
            return ( (_context.Usuarios != null) ? View("Index", new UsuarioViewModel
            {
                Origen = false,
                Paginacion = await Paginacion<Usuarios>.CrearPaginacion(_context.Usuarios.Where(u => u.IdRol == 2).OrderBy(u => u.Apellido).AsNoTracking(), 1, cantidadDeRegistros)
            }) : Problem("Entity set 'OhmydogdbContext.Usuarios'  is null."));
        }

        [HttpGet]
        public async Task<IActionResult> listarUsuarios(string query, int? numeroPagina)
        {
            var usuarios = from usuario in _context.Usuarios select usuario;

            if (query != null && numeroPagina == null){
                numeroPagina = 1;
            }
            if (!String.IsNullOrEmpty(query))
            {
                usuarios = usuarios.Where(u => u.Apellido.Contains(query));
            }

            usuarios = usuarios.Where(u => u.IdRol == 2).OrderBy(u => u.Apellido);

            UsuarioViewModel modelo = new UsuarioViewModel
            {
                Origen = (query != null) ? true : false,
                Paginacion = await Paginacion<Usuarios>.CrearPaginacion(usuarios.AsNoTracking(), numeroPagina ?? 1, cantidadDeRegistros)
            };
            return (PartialView("_ListarUsuarios", modelo));
        }

        public IActionResult CambiarContrasena()
        {
            ViewBag.ActiveView = "CambiarContrasena";
            return (View());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpPost]
        public async Task<JsonResult> Detalles(string id)
        {
            int idUsuario = Convert.ToInt32(id);
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == idUsuario);
            return (Json(usuario));
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Nombre,Apellido,Direccion,Telefono,Estado,Rol,Pass")] Usuarios usuario)
        {
            if (ModelState.IsValid)
            {                
                if (EmailExists(usuario.Email))
                {
                    ModelState.AddModelError("Email", "El email ingresado ya está registrado");
                    //return View(usuario);
                    return RedirectToAction(nameof(Index));
                }
                TempData["RegistroExitoso"] = true;
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Nombre,Apellido,Direccion,Telefono,Estado,Rol,Pass")] Usuarios usuario)
        {

            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (EmailExists(usuario.Email))
                {
                    ModelState.AddModelError("Email", "El email ingresado ya está registrado");
                    return View(usuario);
                }

                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        [HttpPost]
        public async Task<JsonResult> Registrar([FromBody] Usuarios user)
        {
            if (EmailExists(user.Email))
            {
                return (Json(new { success = false, message = "El email ingresado ya está registrado!" }));
            }
            user.Estado = 1;
            user.IdRol = 2;
            user.Pass = GeneradorRandomContrasena();
			_ = EnviarCorreoUsuario(user);
            _context.Add(user);
            await _context.SaveChangesAsync();

            return (Json(new { success = true, message = "Un nuevo cliente ha sido registrado con éxito!" }));
        }

        [HttpPost]
        public async Task<JsonResult> Modificar([FromBody] Usuarios user)
        {
            var _usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (_usuario != null)
            {
                if (_usuario.Email != user.Email)
                {
                    if (EmailExists(user.Email))
                    {
                        return (Json(new { success = false, message = "El email ingresado ya está registrado!" }));
                    }
                }
                try
                {
                    _context.Entry(_usuario).State = EntityState.Detached;
                    _context.Attach(user);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return (Json(new { success = false, message = "Ah ocurrido un error!" }));
                }

                return (Json(new { success = true, message = "El cliente ha sido modificado con éxito!" }));
            }
            return (Json(new { success = false, message = "El usuario no existe!" }));
        }

        [HttpPost]
        public async Task<JsonResult> CambiarPass(string email, string passActual, string newPass, string newPassAgain){
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            if (user!.Pass == passActual)
            {
                if (newPass == newPassAgain){

                    if(!(newPass.Length >= 8))
                        return (Json(new { success = false, message = "La nueva contraseña debe poseer al menos 8 caracteres!" }));
                    if (!Regex.IsMatch(newPass, @"\d"))
                        return (Json(new { success = false, message = "La nueva contraseña debe poseer al menos 1 caracter numérico!" }));
                    if (!Regex.IsMatch(newPass, @"[a-zA-Z]"))
                        return (Json(new { success = false, message = "La nueva contraseña debe poseer al menos 1 caracter alfabético!" }));
                    if (!Regex.IsMatch(newPass, @"\W"))
                        return (Json(new { success = false, message = "La nueva contraseña debe poseer al menos 1 caracter no alfanumérico (caracter especial)!" }));
                    
                    user.Pass = newPass;
                     _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Actualizar el valor del claim de identidad en caso de que el cliente sea nuevo y haya cambiado la pass
                    var actualUser = HttpContext.User;
                    var actualClaim = User.FindFirst("New");
                    if (actualClaim != null && actualClaim.Value == "true")
                    {
                        var nuevoClaim = new Claim("New", "false");
                        var claimsIdentity = (ClaimsIdentity)actualUser.Identity!;
                        claimsIdentity.RemoveClaim(actualClaim);
                        claimsIdentity.AddClaim(nuevoClaim);
                        await HttpContext.SignInAsync(actualUser);

                    }
                    return (Json(new { success = true, message = "La contraseña ha sido actualizada con éxito!" }));
                }
                return (Json(new { success = false, message = "La nueva contraseña no coincide con la repetición de la nueva contraseña!" }));
            }

            return (Json(new { success = false, message = "La contraseña actual no coincide!" }));
        }
        
        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'OhmydogdbContext.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string GeneradorRandomContrasena()
        {
            const int length = 8;
            const string caracteresValidos = "1234567890" +
                                             "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                             "!?@#$%^&*[]{}:;`~()-_+<>,./|";
            var random = new Random();
            var password = new StringBuilder();

            // Agregar caracter '?' de uso booleano para obligar cambio de contrasena en nuevo registro
            password.Append('?');

            // Agregar al menos un carácter numérico
            password.Append(caracteresValidos[random.Next(0, 10)]);

            // Agregar al menos un carácter alfabético
            password.Append(caracteresValidos[random.Next(10, 61)]);

            // Agregar al menos un carácter especial
            password.Append(caracteresValidos[random.Next(61, caracteresValidos.Length)]);

            // Generar los caracteres restantes
            for (int i = 0; i < length - 4; i++)
            {
                password.Append(caracteresValidos[random.Next(caracteresValidos.Length)]);
            }

            // Mezclar los caracteres aleatoriamente
            for (int i = 1; i < length; i++)
            {
                int swapIndex = random.Next(1, 8);
                char temp = password[i];
                password[i] = password[swapIndex];
                password[swapIndex] = temp;
            }

            return (password.ToString());
        }

		public async Task EnviarCorreoUsuario(Usuarios destinatario)
		{
            await Task.Run(() =>
            {
                string remitente = "ohmydoglem@gmail.com",
                       asunto = "OhMyDog - Confirmación de registro";
                StringBuilder cuerpo = new StringBuilder();
                string pathImagen = @"C:\Users\franc\Documents\GitHub\UNLP\Tercer año\1er Semestre\ING 2 - Ingenieria de software 2\Proyecto - Oh my dog!\Oh-my-dog\Oh my dog!\Aplicacion\wwwroot\img\Logo - Veterinaria.png";

                cuerpo.AppendLine("Bienvenido " + (destinatario.Nombre + " " + destinatario.Apellido) + " a nuestra comunidad canina OhMyDog.<br>");
                cuerpo.AppendLine("Su contraseña actual proporcionada es: <strong>" + destinatario.Pass + "</strong>.<br>");
                cuerpo.AppendLine("Para poder utilizar de nuestros servicios se le solicitará actualizar su contraseña al momento de iniciar sesión en el sistema.");
                cuerpo.AppendLine("Saludos cordiales, Pedro y Lucia.");
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("", remitente)); // Correo de origen, tine que estar configurado en el metodo client.Authenticate()
                    message.To.Add(new MailboxAddress("", destinatario.Email)); // Correo de destino
                    message.Subject = asunto;

                    var bodyBuilder = new BodyBuilder();
                    //bodyBuilder.HtmlBody = cuerpo.ToString();
                    bodyBuilder.HtmlBody = "<p>" + cuerpo.ToString() + "</p><img src=\"cid:imagen\">"; // Referencia a la imagen utilizando el ID "imagen"
                                                                                                       // Adjuntar la imagen al correo
                    var imagen = bodyBuilder.LinkedResources.Add(pathImagen);
                    imagen.ContentId = "imagen";
                    message.Body = bodyBuilder.ToMessageBody();

                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        client.Connect("sandbox.smtp.mailtrap.io", 587, false);
                        client.Authenticate("c2bc0d934273d1", "51d937a6997fcb");
                        client.Send(message);
                        client.Disconnect(true);
                    }
                    Console.WriteLine("El correo fue enviado exitosamente!");
                }
                catch (Exception ex)
                {
                    // Manejo de errores aquí
                    Console.WriteLine(ex.Message);
                }
            });
        }

		private bool UsuarioExists(int id)
        {
          return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool EmailExists(string email)
        {
            return (_context.Usuarios?.Any(u => u.Email == email)).GetValueOrDefault();
        }

        private bool validarContrasena(string pass)
        {
            return (
                    (pass.Length >= 8) && (Regex.IsMatch(pass, @"\d")) && (Regex.IsMatch(pass, @"[a-zA-Z]")) && (Regex.IsMatch(pass, @"\W"))
                );
        }
    }
}
