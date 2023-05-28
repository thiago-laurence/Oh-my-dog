using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Models;
using System.Web;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Nancy.Json;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;

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
            return _context.Cuidadores != null ?
                        View(await _context.Cuidadores.ToListAsync()) :
                        Problem("Entity set 'OhmydogdbContext.Cuidadores'  is null.");
        }

        // GET: Cuidadores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cuidadores == null)
            {
                return NotFound();
            }

            var cuidadore = await _context.Cuidadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuidadore == null)
            {
                return NotFound();
            }

            return View(cuidadore);
        }

        // GET: Cuidadores/Create
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Insertar(Cuidadore cuidadore)
        {

            
           
               
               
                    _context.Cuidadores.Add(cuidadore);
                    await _context.SaveChangesAsync();
                    return Json(true);
           
            
        }
        /*_context.Add(cuidadore);
                await _context.SaveChangesAsync();
                return Json(true);
            
            
        }*/

        // POST: Cuidadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Email,HorarioIn,HorarioOut,Foto,Latitud,Longitud,Ubicacion")] Cuidadore cuidadore)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cuidadore);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cuidadore);
        }

        // GET: Cuidadores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cuidadores == null)
            {
                return NotFound();
            }

            var cuidadore = await _context.Cuidadores.FindAsync(id);
            if (cuidadore == null)
            {
                return NotFound();
            }
            return View(cuidadore);
        }

        // POST: Cuidadores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Email,HorarioIn,HorarioOut,Foto,Latitud,Longitud,Ubicacion")] Cuidadore cuidadore)
        {
            if (id != cuidadore.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cuidadore);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuidadoreExists(cuidadore.Id))
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
            return View(cuidadore);
        }

        // GET: Cuidadores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cuidadores == null)
            {
                return NotFound();
            }

            var cuidadore = await _context.Cuidadores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuidadore == null)
            {
                return NotFound();
            }

            return View(cuidadore);
        }



        public async Task<IActionResult> Modificar(int id)
        {
            var contextOptions = new DbContextOptionsBuilder<OhmydogdbContext>()
    .UseSqlServer(@"server=localhost; database=ohmydogdb;integrated security=true; TrustServerCertificate=true;")
    .Options;
            using (var _context = new OhmydogdbContext(contextOptions))
            {



                if (id == null || _context.Cuidadores == null)
                {
                    return NotFound();
                }

                var cuidadore = await _context.Cuidadores
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (cuidadore == null)
                {
                    return NotFound();
                }

                return View(cuidadore);
            }
        }



        [HttpPost]
        public async Task<IActionResult> ModificarFinal(Cuidadore cuidador)
        {


            var borrado = _context.Cuidadores.FirstOrDefault(m => m.Id == cuidador.Id); 
                _context.Cuidadores.Remove(borrado);
                _context.Cuidadores.Add(cuidador);
                await _context.SaveChangesAsync();
                int lastProductId = _context.Cuidadores.Max(item => item.Id);
                return Json(lastProductId);


            



        }

        [HttpGet]
        public string obtenerCuidadores()
        {

            var contextOptions= new DbContextOptionsBuilder<OhmydogdbContext>()
    .UseSqlServer(@"server=localhost; database=ohmydogdb;integrated security=true; TrustServerCertificate=true;")
    .Options;
            using (var db = new OhmydogdbContext(contextOptions))
            {

                return JsonSerializer.Serialize(db.Cuidadores.ToList());
            }
        }

        [HttpPost]
        public async Task<ActionResult> existeCuidador(Cuidadore cuidador)
        {
            
            Cuidadore _cuidador = await _context.Cuidadores.FirstOrDefaultAsync(m => m.Email == cuidador.Email && m.Ubicacion== cuidador.Ubicacion);

            if (_cuidador != null)
            {
                return Json(true);
            }
            

            return Json(false);
        }
        // POST: Cuidadores/Delete/5

        [HttpPost]
        public async Task<ActionResult> borrarCuidador(int id)
        {
            var contextOptions =new DbContextOptionsBuilder<OhmydogdbContext>()
    .UseSqlServer(@"server=localhost; database=ohmydogdb;integrated security=true; TrustServerCertificate=true;")
    .Options;
            using (var db = new OhmydogdbContext(contextOptions))
            {   
                var cuidador = await db.Cuidadores.FirstOrDefaultAsync(m => m.Id == id);
                db.Cuidadores.Remove(cuidador);
                await db.SaveChangesAsync();
                return Json(true);
            }
            
        }



        [HttpPost, ActionName("Delete")]
       
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cuidadores == null)
            {
                return Problem("Entity set 'OhmydogdbContext.Cuidadores'  is null.");
            }
            var cuidadore = await _context.Cuidadores.FindAsync(id);
            if (cuidadore != null)
            {
                _context.Cuidadores.Remove(cuidadore);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }






        public IActionResult EnviarCorreo(string remitente, string asunto, string contenido, string destino)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("", remitente)); // Correo de origen, tine que estar configurado en el metodo client.Authenticate()
                message.To.Add(new MailboxAddress("", "lautaromoller345@gmail.com")); // Correo de destino
                message.Subject = asunto;

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

                return RedirectToAction("Index", "Home"); // o redirige a la página que desees después de enviar el correo electrónico
            }
            catch (Exception ex)
            {
                // Manejo de errores aquí
                return RedirectToAction("Error", "Home");
            }
        }





        private bool CuidadoreExists(int id)
        {
          return (_context.Cuidadores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
