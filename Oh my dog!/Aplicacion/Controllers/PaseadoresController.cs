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

            var paseadore = await _context.Paseadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paseadore == null)
            {
                return NotFound();
            }

            return View(paseadore);
        }

        // GET: Paseadores/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Insertar(Paseadore paseadore)
        {


            
                _context.Paseadores.Add(paseadore);
            await _context.SaveChangesAsync();
            return Json(true);
            

        }
        /*_context.Add(paseadore);
                await _context.SaveChangesAsync();
                return Json(true);
            
            
        }*/

        // POST: Paseadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Email,HorarioIn,HorarioOut,Foto,Latitud,Longitud,Ubicacion")] Paseadore paseadore)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paseadore);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(paseadore);
        }

        // GET: Paseadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Paseadores == null)
            {
                return NotFound();
            }

            var paseadore = await _context.Paseadores.FindAsync(id);
            if (paseadore == null)
            {
                return NotFound();
            }
            return View(paseadore);
        }

        // POST: Paseadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Email,HorarioIn,HorarioOut,Foto,Latitud,Longitud,Ubicacion")] Paseadore paseadore)
        {
            if (id != paseadore.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paseadore);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaseadoreExists(paseadore.Id))
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
            return View(paseadore);
        }

        // GET: Paseadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Paseadores == null)
            {
                return NotFound();
            }

            var paseadore = await _context.Paseadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paseadore == null)
            {
                return NotFound();
            }

            return View(paseadore);
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
            }
            return Json(result);
        }

        public String getActualPath(String filename)
        {
            return Path.Combine(Path.GetFullPath("wwwroot")+"\\img", filename);
        }

        public async Task<IActionResult> Modificar(int id)
        {
            var contextOptions = new DbContextOptionsBuilder<OhmydogdbContext>()
    .UseSqlServer(@"server=localhost; database=ohmydogdb;integrated security=true; TrustServerCertificate=true;")
    .Options;
            using (var _context = new OhmydogdbContext(contextOptions))
            {



                if (id == null || _context.Paseadores == null)
                {
                    return NotFound();
                }

                var paseadore = await _context.Paseadores
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (paseadore == null)
                {
                    return NotFound();
                }

                return View(paseadore);
            }
        }



        [HttpPost]
        public async Task<IActionResult> ModificarFinal(Paseadore paseador)
        {


            var borrado = _context.Paseadores.FirstOrDefault(m => m.Id == paseador.Id);
            _context.Paseadores.Remove(borrado);
            _context.Paseadores.Add(paseador);
            await _context.SaveChangesAsync();
            int lastProductId = _context.Paseadores.Max(item => item.Id);
            return Json(lastProductId);






        }

        [HttpGet]
        public string obtenerPaseadores()
        {




            return JsonSerializer.Serialize(_context.Paseadores.ToList());

        }

        [HttpPost]
        public async Task<ActionResult> existePaseador(Paseadore paseador)
        {

            Paseadore _paseador = await _context.Paseadores.FirstOrDefaultAsync(m => m.Email == paseador.Email && m.Ubicacion == paseador.Ubicacion);

            if (_paseador != null)
            {
                return Json(true);
            }


            return Json(false);
        }
        // POST: Paseadores/Delete/5

        [HttpPost]
        public async Task<ActionResult> borrarPaseador(int id)
        {
            
                var paseador = await _context.Paseadores.FirstOrDefaultAsync(m => m.Id == id);
                _context.Paseadores.Remove(paseador);
                await _context.SaveChangesAsync();
                return Json(true);
            

        }



        [HttpPost, ActionName("Delete")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Paseadores == null)
            {
                return Problem("Entity set 'OhmydogdbContext.Paseadores'  is null.");
            }
            var paseadore = await _context.Paseadores.FindAsync(id);
            if (paseadore != null)
            {
                _context.Paseadores.Remove(paseadore);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CorreoExitoso()
        {
            return View();
        }





        public async Task<IActionResult> SendEmail(string origen, string destino, string titulo, string mensaje)
        {
            var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("753b469e9e376d", "06af1e23c346ae"),
                EnableSsl = true
            };
            client.Send(origen, destino, titulo, mensaje);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult EnviarCorreo(string remitente, string asunto, string contenido, string destinatario)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("", "ohmydog_oficial@outlook.es")); // Correo de origen, tine que estar configurado en el metodo client.Authenticate()
                message.To.Add(new MailboxAddress("", "lautaromoller345@gmail.com")); // Correo de destino
                message.Subject = asunto;
                contenido = contenido + "<br/>"+ "<br/>"+ "<br/>" + "Te dejo mi mail: " + remitente;
                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = contenido;
                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
                    client.Authenticate("ohmydog_oficial@outlook.es", "zKbP.-6rQT:i4JE");
                    client.Send(message);
                    client.Disconnect(true);
                }

                return RedirectToAction("CorreoExitoso", "Paseadores"); // o redirige a la página que desees después de enviar el correo electrónico
            }
            catch (Exception ex)
            {
                // Manejo de errores aquí
                return RedirectToAction("Error", "Home");
            }
        }



        private bool PaseadoreExists(int id)
        {
            return (_context.Paseadores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
