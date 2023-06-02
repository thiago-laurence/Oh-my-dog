using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Models;
using System.Text.Json;
using MimeKit;

namespace Aplicacion.Controllers
{
    public class CuidadoresController : Controller
    {
        private readonly OhmydogdbContext _context;

        public CuidadoresController(OhmydogdbContext context)
        {
            _context = context;
        }

        // GET: Cuidadores
        public async Task<IActionResult> Index()
        {
            ViewBag.ActiveView = "Cuidadores";
            return _context.Cuidadores != null ? 
                          View(await _context.Cuidadores.ToListAsync()) :
                          Problem("Entity set 'OhmydogdbContext.Cuidadores'  is null.");
        }

        [HttpPost]
        public async Task<IActionResult> Insertar(Cuidadores cuidador)
        {
            _context.Cuidadores.Add(cuidador);
            await _context.SaveChangesAsync();

            return (Json(new { success = true, message = "Un nuevo cuidador ha sido registrado con éxito!" }));
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
                using (FileStream stream = System.IO.File.Create(imagePath))
                {
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
            return Path.Combine(Path.GetFullPath("wwwroot") + "\\img", filename);
        }

        [HttpPost]
        public async Task<IActionResult> ModificarFinal(Cuidadores cuidador)
        {
            var borrado = _context.Cuidadores.FirstOrDefault(m => m.Id == cuidador.Id);
            if (borrado != null){
                _context.Cuidadores.Remove(borrado);
                _context.Cuidadores.Add(cuidador);
                await _context.SaveChangesAsync();
            }
            return (Json(new { success = true, message = "El cuidador ha sido modificado con éxito!" }));
        }

        [HttpGet]
        public string obtenerCuidadores()
        {
            return JsonSerializer.Serialize(_context.Cuidadores.ToList());
        }

        [HttpPost]
        public async Task<ActionResult> existeCuidador(Cuidadores cuidador)
        {
            Cuidadores? _cuidador = await _context.Cuidadores.FirstOrDefaultAsync(m => m.Email == cuidador.Email && m.Ubicacion == cuidador.Ubicacion);

            if (_cuidador != null)
            {
                return (Json(new { success = true, message = "El cuidador con email \"" + cuidador.Email + "\" ya está registrado en la zona!" }));
            }

            return (Json(new { success = false, message = "" }));
        }

        [HttpPost]
        public async Task<ActionResult> borrarCuidador(int id)
        {
            var cuidador = await _context.Cuidadores.FirstOrDefaultAsync(m => m.Id == id);
            if (cuidador != null){
                _context.Cuidadores.Remove(cuidador);
                await _context.SaveChangesAsync();
            }
            return (Json(new { success = true, message = "La publicación del cuidador ha sido eliminada con éxito!" }));
        }

        [HttpPost]
        public JsonResult ContactarCuidador(string remitente, string asunto, string contenido, string destinatario)
        {
            _ = EnviarCorreo(remitente, asunto, contenido, destinatario);

            return (Json(new { success = true, message = "El correo fue enviado al cuidador con éxito!" }));
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
        public async Task<JsonResult> ObtenerCuidador(string id)
        {
            int idCuidador = Convert.ToInt32(id);
            var cuidador = await _context.Cuidadores.FirstOrDefaultAsync(m => m.Id == idCuidador);
            return (Json(cuidador));
        }

        // GET: Cuidadores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cuidadores == null)
            {
                return NotFound();
            }

            var cuidadores = await _context.Cuidadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuidadores == null)
            {
                return NotFound();
            }

            return View(cuidadores);
        }

        // GET: Cuidadores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cuidadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Email,HorarioIn,HorarioOut,Foto,Latitud,Longitud,Ubicacion")] Cuidadores cuidadores)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cuidadores);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cuidadores);
        }

        // GET: Cuidadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cuidadores == null)
            {
                return NotFound();
            }

            var cuidadores = await _context.Cuidadores.FindAsync(id);
            if (cuidadores == null)
            {
                return NotFound();
            }
            return View(cuidadores);
        }

        // POST: Cuidadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Email,HorarioIn,HorarioOut,Foto,Latitud,Longitud,Ubicacion")] Cuidadores cuidadores)
        {
            if (id != cuidadores.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cuidadores);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuidadoresExists(cuidadores.Id))
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
            return View(cuidadores);
        }

        // GET: Cuidadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cuidadores == null)
            {
                return NotFound();
            }

            var cuidadores = await _context.Cuidadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuidadores == null)
            {
                return NotFound();
            }

            return View(cuidadores);
        }

        // POST: Cuidadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cuidadores == null)
            {
                return Problem("Entity set 'OhmydogdbContext.Cuidadores'  is null.");
            }
            var cuidadores = await _context.Cuidadores.FindAsync(id);
            if (cuidadores != null)
            {
                _context.Cuidadores.Remove(cuidadores);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CuidadoresExists(int id)
        {
          return (_context.Cuidadores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
