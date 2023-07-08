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
using System.Security.Claims;

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
        public IActionResult Sugerencias(int idPerro)
        {
            ViewBag.ActiveView = "Tinderdog";
            var miPerro = _context.PublicacionTinderdog.Where(p => p.IdPerro == idPerro).Include(p => p.Perro).Include(p => p.MeGustaDados).Include(p => p.NoMeGustaDados).First();

            var sugerencias = _context.PublicacionTinderdog.Where(pb => pb.IdPerro != miPerro.Perro.Id).Include(pb => pb.Perro).Include(pb => pb.Fotos)
            .Where(pb => (pb.Perro.IdDueno != miPerro.Perro.IdDueno) && (pb.Perro.Sexo != miPerro.Perro.Sexo) && !(miPerro.MeGustaDados.Select(m => m.IdPerroReceptor).Contains(pb.Perro.Id)) && !(miPerro.NoMeGustaDados.Select(m => m.IdPerroReceptor).Contains(pb.Perro.Id)));

            var ordenamiento = sugerencias.OrderByDescending(pb => pb.Perro.Raza == miPerro.Perro.Raza && pb.Perro.Color == miPerro.Perro.Color)
                                          .ThenByDescending(pb => pb.Perro.Raza == miPerro.Perro.Raza)
                                          .ThenByDescending(pb => pb.Perro.Color == miPerro.Perro.Color);

            return (View("Sugerencias", new TinderdogViewModel { Perro = miPerro.Perro, Publicaciones = ordenamiento.Take(2).ToList() }));
        }

        [HttpGet]
        public IActionResult ListarSugerencias(int idPerro)
        {
            var miPerro = _context.PublicacionTinderdog.Where(p => p.IdPerro == idPerro).Include(p => p.Perro).Include(p => p.MeGustaDados).Include(p => p.NoMeGustaDados).First();

            var sugerencias = _context.PublicacionTinderdog.Where(pb => pb.IdPerro != miPerro.Perro.Id).Include(pb => pb.Perro).Include(pb => pb.Fotos)
            .Where(pb => (pb.Perro.IdDueno != miPerro.Perro.IdDueno) && (pb.Perro.Sexo != miPerro.Perro.Sexo) && !(miPerro.MeGustaDados.Select(m => m.IdPerroReceptor).Contains(pb.Perro.Id)) && !(miPerro.NoMeGustaDados.Select(m => m.IdPerroReceptor).Contains(pb.Perro.Id)));

            var ordenamiento = sugerencias.OrderByDescending(pb => pb.Perro.Raza == miPerro.Perro.Raza && pb.Perro.Color == miPerro.Perro.Color)
                                          .ThenByDescending(pb => pb.Perro.Raza == miPerro.Perro.Raza)
                                          .ThenByDescending(pb => pb.Perro.Color == miPerro.Perro.Color);

            return (PartialView("_ListarSugerencias", ordenamiento.Take(2).ToList()));
        }

        public IActionResult MisCandidatos()
        {
            ViewBag.ActiveView = "Tinderdog";
            string? email = User.FindFirst("Email")?.Value;
            var misPerros = _context.Usuarios.Where(u => u.Email == email).Include(u => u.GetPerros.Where(p => p.Estado == true))
                            .ThenInclude(p => p.PublicacionTinderdog).First().GetPerros.OrderBy(p => p.Nombre).ToList();
            return _context.Perros != null ?
                          View("MisCandidatos", misPerros) :
                          Problem("Entity set 'OhmydogdbContext.Perros'  is null.");
        }

        [HttpGet]
        public IActionResult ListarMisCandidatos()
        {
            string? email = User.FindFirst("Email")?.Value;
            var misPerros = _context.Usuarios.Where(u => u.Email == email).Include(u => u.GetPerros.Where(p => p.Estado == true))
                            .ThenInclude(p => p.PublicacionTinderdog).First().GetPerros.OrderBy(p => p.Nombre).ToList();
            return (PartialView("_ListarMisCandidatos", misPerros));
        }

        public IActionResult MeGustasRecibidos(int idPerro)
        {
            ViewBag.ActiveView = "Tinderdog";
            var perro = _context.PublicacionTinderdog.Where(p => p.IdPerro == idPerro)
                           .Include(p => p.Perro).Include(p => p.MeGustaDados).Include(p => p.MeGustaRecibidos).Include(p => p.NoMeGustaDados).First();
            var matches = perro.MeGustaDados.Join(perro.MeGustaRecibidos, e => e.IdPerroReceptor, r => r.IdPerroEmisor, (e, r) => r.IdPerroEmisor);
            var meGustas = _context.PerrosMeGusta.Where(m => m.IdPerroReceptor == perro.IdPerro && !matches.Any(i => i == m.IdPerroEmisor) && !perro.NoMeGustaDados.Select(p => p.IdPerroReceptor).Contains(m.IdPerroEmisor))
                           .Include(p => p.PerroEmisor)
                           .ThenInclude(p => p.Perro).Include(p => p.PerroEmisor.Fotos).Select(p => p.PerroEmisor).ToList();

            return View("MeGustasRecibidos", new TinderdogViewModel() { Perro = perro.Perro, Publicaciones = meGustas });
        }

        [HttpGet]
        public IActionResult ListarMeGustasRecibidos(int idPerro)
        {
            var perro = _context.PublicacionTinderdog.Where(p => p.IdPerro == idPerro)
                           .Include(p => p.MeGustaDados).Include(p => p.MeGustaRecibidos).Include(p => p.NoMeGustaDados).First();
            var matches = perro.MeGustaDados.Join(perro.MeGustaRecibidos, e => e.IdPerroReceptor, r => r.IdPerroEmisor, (e, r) => r.IdPerroEmisor);
            var meGustas = _context.PerrosMeGusta.Where(m => m.IdPerroReceptor == perro.IdPerro && !matches.Any(i => i == m.IdPerroEmisor) && !perro.NoMeGustaDados.Select(p => p.IdPerroReceptor).Contains(m.IdPerroEmisor))
                           .Include(p => p.PerroEmisor)
                           .ThenInclude(p => p.Perro).Include(p => p.PerroEmisor.Fotos).Select(p => p.PerroEmisor).ToList();

            return PartialView("_ListarMeGustasRecibidos", meGustas);
        }

        [HttpPost]
        public JsonResult MeGusta(int idPerroEmisor, int idPerroReceptor)
        {
            _context.PerrosMeGusta.Add(new PerrosMeGusta { IdPerroEmisor = idPerroEmisor, IdPerroReceptor = idPerroReceptor });
            _context.SaveChanges();

            var match = _context.PerrosMeGusta.Where(p => p.IdPerroEmisor == idPerroReceptor).Any(m => m.IdPerroReceptor == idPerroEmisor);
            if (match)
            {
                var perroReceptor = _context.Perros.Where(p => p.Id == idPerroReceptor).Include(p => p.Dueno).First();
                var perroEmisor = _context.Perros.Where(p => p.Id == idPerroEmisor).Include(p => p.Dueno).First();
                string asunto = "Se ha producido un ¡Flechazo!";

                string contenido = "Se le informa que su perro " + perroEmisor.Nombre + " ha logrado un flechazo con el perro " + perroReceptor.Nombre + ". " +
                    "Ponte en contacto lo antes posible y ¡coordina una cita! <br> Atentamente el equipo de 'OhMyDog'.";
                _ = EnviarCorreoMatch("ohmydog@gmail.com", perroEmisor.Dueno.Email, asunto, contenido);

                contenido = "Se le informa que su perro " + perroReceptor.Nombre + " ha logrado un flechazo con el perro " + perroEmisor.Nombre + ". " +
                    "Ponte en contacto lo antes posible y ¡coordina una cita! <br> Atentamente el equipo de 'OhMyDog'.";
                _ = EnviarCorreoMatch("ohmydog@gmail.com", perroReceptor.Dueno.Email, asunto, contenido);

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

        public IActionResult Matches(int idPerro)
        {
            var perro = _context.PublicacionTinderdog.Where(p => p.IdPerro == idPerro).Include(p => p.Perro).Include(p => p.MeGustaDados).Include(p => p.MeGustaRecibidos).First();
            var matches = perro.MeGustaDados.Join(perro.MeGustaRecibidos, e => e.IdPerroReceptor, r => r.IdPerroEmisor, (e, r) => e.IdPerroReceptor)
                                            .Join(_context.PublicacionTinderdog.Include(p => p.Perro).Include(p => p.Fotos), id => id, p => p.IdPerro, (id, p) => p).ToList();

            return (View("Matches", new TinderdogViewModel { Perro = perro.Perro, Publicaciones = matches }));
        }

        public async Task EnviarCorreoMatch(string remitente, string destinatario, string asunto, string contenido)
        {
            await Task.Run(() =>
            {
                try
                {
                    string pathImagen = @"wwwroot\img\tinderdog - logo 2.png";
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
            var publicacion = _context.PublicacionTinderdog.FirstOrDefault(p => p.IdPerro == idPerro);
            if (publicacion != null)
            {
                List<FotosPublicacionTinderdog> fotos = _context.FotosPublicacionTinderdog.Where(id => id.IdPublicacion == publicacion.Id).ToList();
                _context.FotosPublicacionTinderdog.RemoveRange(fotos);
                _context.PublicacionTinderdog.Remove(publicacion);
                _context.SaveChanges();
            }

            return (Json(new { success = true }));
        }

        [HttpPost]
        public JsonResult PublicarCandidato(PublicacionTinderdog publicacion, List<string> fotos)
        {
            _context.PublicacionTinderdog.Add(publicacion);
            if (fotos[0] != null)
            {
                _context.SaveChanges();
                var maxId = _context.PublicacionTinderdog.Max(p => p.Id);
                fotos.ForEach(f => {
                    _context.FotosPublicacionTinderdog.Add(new FotosPublicacionTinderdog { IdPublicacion = maxId, Foto = f });
                });
            }

            _context.SaveChanges();

            return (Json(new { success = true }));
        }

        [HttpPost]
        public async Task<ActionResult> CargarFoto()
        {
            bool result = false;
            IFormFile archivo = Request.Form.Files.First();
            string fileName = archivo.Name;
            string imagePath = GetActualPath(fileName);
            try
            {
                if (System.IO.File.Exists(imagePath))
                {

                    System.IO.File.Delete(imagePath);
                }
                using (FileStream stream = System.IO.File.Create(imagePath))
                {
                    await archivo.CopyToAsync(stream);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(result);
        }
        public String GetActualPath(String filename)
        {
            return Path.Combine(Path.GetFullPath("wwwroot") + "\\img", filename);
        }
    }
}
