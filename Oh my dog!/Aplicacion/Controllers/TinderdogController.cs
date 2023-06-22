using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Models;
using NuGet.Versioning;
using MimeKit;

namespace Aplicacion.Controllers
{
    public class TinderdogController : Controller
    {
        private readonly OhmydogdbContext _context;

        public TinderdogController(OhmydogdbContext context)
        {
            _context = context;
        }

        // GET: Tinderdog
        public IActionResult Index()
        {
            ViewBag.ActiveView = "Tinderdog";
            ViewBag.SubView = "Sugerencias";
            string? email = User.FindFirst("Email")?.Value;
            return _context.Perros != null ?
                          View(_context.Usuarios.Where(u => u.Email == email).Include(u => u.GetPerros.Where(p => p.Estado == true)).First().GetPerros.OrderBy(p => p.Nombre).ToList()) :
                          Problem("Entity set 'OhmydogdbContext.Perros'  is null.");
        }

        public IActionResult MisCandidatos()
        {
            ViewBag.ActiveView = "Tinderdog";
            ViewBag.SubView = "MisCandidatos";
            string? email = User.FindFirst("Email")?.Value;
            return _context.Perros != null ?
                          View("MisCandidatos", _context.Usuarios.Where(u => u.Email == email).Include(u => u.GetPerros.Where(p => p.Estado == true)).First().GetPerros.OrderBy(p => p.Nombre).ToList()) :
                          Problem("Entity set 'OhmydogdbContext.Perros'  is null.");
        }

        public IActionResult MeGustasRecibidos(int idPerro)
        {
            ViewBag.ActiveView = "Tinderdog";
            ViewBag.SubView = "MisCandidatos";
            var perro = _context.Perros.Where(p => p.Id == idPerro).FirstOrDefault();
            var meGustas = _context.PerrosMeGusta.Where(m => m.IdPerroReceptor == idPerro).Include(m => m.PerroReceptor).Select(m => m.PerroEmisor).ToList();
            return View("MeGustasRecibidos", new MeGustasRecibidosViewModel() { Perro = perro, GetMeGustas = meGustas });
        }


        [HttpGet]
        public IActionResult ListarSugerencias(int idPerro)
        {
            var perro = _context.Perros.Where(p => p.Id == idPerro).Include(p => p.MeGustaDados).Include(p => p.NoMeGustaDados).FirstOrDefault();
            var sugerencias = _context.Perros.Where(p => (p.IdDueno != perro!.IdDueno) && (p.Sexo != perro.Sexo) && !(perro.MeGustaDados.Select(m => m.IdPerroReceptor).Contains(p.Id)) && !(perro.NoMeGustaDados.Select(m => m.IdPerroReceptor).Contains(p.Id))); 
            var ordenamiento = sugerencias.OrderByDescending(p => p.Raza == perro!.Raza && p.Color == perro.Color)
                                          .ThenByDescending(p => p.Raza == perro!.Raza)
                                          .ThenByDescending(p => p.Color == perro!.Color);
            return (PartialView("_ListarSugerencias", ordenamiento.Take(2).ToList()));
        }

        [HttpPost]
        public JsonResult MeGusta(int idPerroEmisor, int idPerroReceptor)
        {
            _context.PerrosMeGusta.Add(new PerrosMeGusta { IdPerroEmisor = idPerroEmisor, IdPerroReceptor = idPerroReceptor });
            _context.SaveChanges();

            var match = _context.PerrosMeGusta.Where(p => p.IdPerroEmisor == idPerroReceptor).Any(m => m.IdPerroReceptor == idPerroEmisor);
            if (match)
            {
                return (Json(new { match = true }));
            }

            return (Json(new { match = false }));
        }

        [HttpPost]
        public JsonResult NoMeGusta(int idPerroEmisor, int idPerroReceptor)
        {
            _context.PerrosNoMeGusta.Add(new PerrosNoMeGusta { IdPerroEmisor = idPerroEmisor, IdPerroReceptor = idPerroReceptor });
            _context.SaveChanges();

            return (Json(new { success = true }));
        }

        [HttpPost]
        public JsonResult RegistrarCelo(int idPerro, DateTime FechaCelo)
        {
            var perro = _context.Perros.Where(p => p.Id == idPerro).FirstOrDefault();
            perro!.Celo = FechaCelo;
            _context.Update(perro);
            _context.SaveChanges();

            return ( Json(new { success = true }));
        }

        [HttpGet]
        public JsonResult VisualizarContacto(int idPerro)
        {
            var perro = _context.Perros.Where(p => p.Id == idPerro).Include(p => p.Dueno).FirstOrDefault();
            return (Json(new { success = true, idPerro = perro!.Id, nombrePerro = perro.Nombre, emailDueno = perro.Dueno.Email }));
        }

        [HttpPost]
        public JsonResult ContactarMatch(string remitente, string destinatario, string asunto, string contenido)
        {
            _ = EnviarCorreoMatch(remitente, destinatario, asunto, contenido);

            return (Json(new { success = true }));
        }

        public async Task EnviarCorreoMatch(string remitente, string destinatario, string asunto, string contenido)
        {
            await Task.Run(() =>
            {
                try
                {
                    string pathImagen = @"C:\Users\franc\Documents\GitHub\UNLP\Tercer año\1er Semestre\ING 2 - Ingenieria de software 2\Proyecto - Oh my dog!\Oh-my-dog\Oh my dog!\Aplicacion\wwwroot\img\tinderdog - logo 2.png";
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("", remitente)); // Correo de origen
                    message.To.Add(new MailboxAddress("", destinatario)); // Correo de destino
                    message.Subject = asunto;
                    var bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = "<p>" + contenido + "</p><img src=\"cid:imagen\" style=\"height: 200px;\">";
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

                    Console.WriteLine("¡El correo fue enviado exitosamente!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message); // Mostrar mensaje de error por consola
                }
            });
        }

        [HttpPost]
        public JsonResult QuitarCandidato(int idPerro)
        {
            return (Json(new { success = true }));
        }

        [HttpPatch]
        public JsonResult PublicarCandidato(int idPerro)
        {
            return (Json(new { success = true }));
        }
    }
}
