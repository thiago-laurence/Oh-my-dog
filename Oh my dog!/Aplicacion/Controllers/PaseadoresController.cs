using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Models;
using System.Net.Mail;
using System.Net;
using System.Text.Json;

using MailKit.Security;
using MimeKit;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Hosting.Internal;
using System.Drawing;

namespace Aplicacion.Controllers
{
    public class PaseadoresController : Controller
    {
        private readonly OhmydogdbContext _context;

        public PaseadoresController(OhmydogdbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveView = "Paseadores";
            return _context.Paseadores != null ?
                        View(await _context.Paseadores.ToListAsync()) :
                        Problem("Entity set 'OhmydogdbContext.Paseadores'  is null.");
        }

        // GET: Paseadores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Paseadores == null)
            {
                return NotFound();
            }

            var paseador = await _context.Paseadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paseador == null)
            {
                return NotFound();
            }

            return View(paseador);
        }

        // GET: Paseadores/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Insertar(Paseadores paseador)
        {
            _context.Paseadores.Add(paseador);
            await _context.SaveChangesAsync();
            return (Json(new { success = true, message = "Un nuevo paseador ha sido registrado con éxito!" }));
        }

        // POST: Paseadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Email,HorarioIn,HorarioOut,Foto,Latitud,Longitud,Ubicacion")] Paseadores paseador)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paseador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paseador);
        }

        // GET: Paseadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Paseadores == null)
            {
                return NotFound();
            }

            var paseador = await _context.Paseadores.FindAsync(id);
            if (paseador == null)
            {
                return NotFound();
            }
            return View(paseador);
        }

        // POST: Paseadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Email,HorarioIn,HorarioOut,Foto,Latitud,Longitud,Ubicacion")] Paseadores paseador)
        {
            if (id != paseador.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paseador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaseadorExists(paseador.Id))
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
            return View(paseador);
        }

        // GET: Paseadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Paseadores == null)
            {
                return NotFound();
            }

            var paseador = await _context.Paseadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paseador == null)
            {
                return NotFound();
            }

            return View(paseador);
        }
        [HttpPost]
        public async Task<ActionResult> uploadFile()
        {
            string result = string.Empty;
            IFormFile archivo = Request.Form.Files.First();
            string fileName = archivo.Name;
            string imagePath = getActualPath(fileName);
            try
            {
                if (System.IO.File.Exists(imagePath))
                {

                    System.IO.File.Delete(imagePath);
                }
                using (FileStream stream = System.IO.File.Create(imagePath)){
                    await archivo.CopyToAsync(stream);
                    result = "pass";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(result);
        }

        public String getActualPath(String filename)
        {
            return Path.Combine(Path.GetFullPath("wwwroot")+"\\img", filename);
        }

        public async Task<IActionResult> Modificar(int id)
        {
            var paseador = await _context.Paseadores.FirstOrDefaultAsync(p => p.Id == id);
            if (paseador != null)
            {
                return (View(paseador));
            }
            return (NotFound());

        }



        [HttpPost]
        public async Task<IActionResult> ModificarFinal(Paseadores paseador)
        {
            var borrado = _context.Paseadores.FirstOrDefault(m => m.Id == paseador.Id);
            if (borrado != null){
                _context.Paseadores.Remove(borrado);
                _context.Paseadores.Add(paseador);
                await _context.SaveChangesAsync();
            }
            return (Json(new { success = true, message = "El Paseador ha sido modificado con éxito!" }));
        }

        [HttpGet]
        public string obtenerPaseadores()
        {
            return JsonSerializer.Serialize(_context.Paseadores.ToList());
        }

        [HttpPost]
        public async Task<ActionResult> existePaseador(Paseadores paseador)
        {
            Paseadores? _paseador = await _context.Paseadores.FirstOrDefaultAsync(m => m.Email == paseador.Email && m.Ubicacion == paseador.Ubicacion);

            if (_paseador != null)
            {
                return (Json(new { success = true, message = "El paseador con email \"" + paseador.Email + "\" ya está registrado en la zona!" }));
            }

            return (Json(new { success = false, message = "" }));
        }
        // POST: Paseadores/Delete/5

        [HttpPost]
        public async Task<ActionResult> borrarPaseador(int id)
        {
            var paseador = await _context.Paseadores.FirstOrDefaultAsync(m => m.Id == id);
            if(paseador != null){
                _context.Paseadores.Remove(paseador);
                await _context.SaveChangesAsync();
            }
            return (Json(new { success = true, message = "La publicación del paseador ha sido eliminada con éxito!" }));
        }



        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Paseadores == null)
            {
                return Problem("Entity set 'OhmydogdbContext.Paseadores'  is null.");
            }
            var paseador = await _context.Paseadores.FindAsync(id);
            if (paseador != null)
            {
                _context.Paseadores.Remove(paseador);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public JsonResult ContactarPaseador(string remitente, string asunto, string contenido, string destinatario)
        {
            _ = EnviarCorreo(remitente, asunto, contenido, destinatario);
            
            return (Json(new { success = true, message = "El correo fue enviado al paseador con éxito!" }));
        }





        public IActionResult SendEmail(string origen, string destino, string titulo, string mensaje)
        {
            var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("753b469e9e376d", "06af1e23c346ae"),
                EnableSsl = true
            };
            client.Send(origen, destino, titulo, mensaje);
            return RedirectToAction(nameof(Index));
        }

        
        public async Task EnviarCorreo(string remitente, string asunto, string contenido, string destinatario)
        {
            await Task.Run(() =>
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("", "ohmydoglem@gmail.com")); // Correo de origen, tiene que estar configurado en el metodo client.Authenticate()
                    message.To.Add(new MailboxAddress("", destinatario)); // Correo de destino
                    message.Subject = asunto;
                    contenido = contenido + "<br>" + "<br>" + "<br>" + "El email de la persona que se contactó con usted es: " + remitente;
                    var bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = contenido;
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
                    Console.WriteLine(ex.Message); // Mostrar mensaje de error por consola
                }
            });
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerPaseador(string id)
        {
            int idPaseador = Convert.ToInt32(id);
            var paseador = await _context.Paseadores.FirstOrDefaultAsync(m => m.Id == idPaseador);
            return (Json(paseador));
        }

        private bool EmailExists(string email)
        {
            return (_context.Paseadores?.Any(u => u.Email == email)).GetValueOrDefault();
        }

        private bool PaseadorExists(int id)
        {
            return (_context.Paseadores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
