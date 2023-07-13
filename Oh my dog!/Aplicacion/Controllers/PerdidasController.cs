using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Models;
using Aplicacion.Data;
using MimeKit;
using System.Security.Claims;

namespace Aplicacion.Controllers
{
    public class PerdidasController : Controller
    {
        private readonly OhmydogdbContext _context;

        public PerdidasController(OhmydogdbContext context)
        {
            _context = context;
        }

        public IActionResult PublicarPerdidasIndex()
        {
            ViewBag.ActiveView = "Publicaciones";
            ViewBag.SubView = "PublicarPerdida";
            if (!User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("PublicarPerdidasIndex");
        }

        [HttpPost]
        public JsonResult PublicarPerdidasIndex(Perdidas perdida)
        {

            if (hayAdopcionConEmailYNombre(perdida.Email, perdida.Nombre.ToLower()))
            {
                return (Json(new { success = false, message = "Ya existe una publicacion de perdida para este usuario con ese nombre de perro, por favor elija otro nombre" }));
            }
            perdida.Nombre = perdida.Nombre.ToLower();
            _context.Add(perdida);
            _context.SaveChanges();

            return (Json(new { success = true, message = "El perro ha sido publicado en como perdido con éxito" }));
        }

        // Variable para indicar la cantida a mostrar por la paginacion
        private int cantidadDeRegistros = 4;
        public async Task<IActionResult> IndexPerdidas()
        {
            ViewBag.ActiveView = "Publicaciones";
            ViewBag.SubView = "IndexPerdidas";
            var perdidas = from perdida in _context.Perdidas select perdida;

            if (User.Identity!.IsAuthenticated)
            {
                perdidas = perdidas.Where(a => a.Email != User.FindFirstValue("Email") && a.Baja == 0).OrderBy(a => a.Estado).ThenByDescending(a => a.Id).ThenBy(a => a.Nombre);
            }
            else
            {
                perdidas = perdidas.Where(a => a.Baja == 0).OrderBy(a => a.Estado).ThenByDescending(a => a.Id).ThenBy(a => a.Nombre);
            }

            return ((_context.Perdidas != null) ? View("IndexPerdidas", new PerdidasViewModel
            {
                Origen = false,
                Paginacion = await Paginacion<Perdidas>.CrearPaginacion(perdidas.AsNoTracking(), 1, cantidadDeRegistros)
            }) : Problem("Entity set 'OhmydogdbContext.Usuarios'  is null."));
        }

        [HttpGet]
        public async Task<IActionResult> ListarPerdidas(string query, int? numeroPagina)
        {
            var perdidas = from perdida in _context.Perdidas select perdida;

            if (query != null && numeroPagina == null)
            {
                numeroPagina = 1;
            }

            if (!String.IsNullOrEmpty(query))
            {
                perdidas = perdidas.Where(a => a.Nombre.Contains(query));
            }

            if (User.Identity!.IsAuthenticated)
            {
                perdidas = perdidas.Where(a => a.Email != User.FindFirstValue("Email") && a.Baja == 0).OrderBy(a => a.Estado).ThenByDescending(a => a.Id).ThenBy(a => a.Nombre);
            }
            else
            {
                perdidas = perdidas.Where(a => a.Baja == 0).OrderBy(a => a.Estado).ThenByDescending(a => a.Id).ThenBy(a => a.Nombre);
            }

            PerdidasViewModel modelo = new PerdidasViewModel
            {
                Origen = (query != null) ? true : false,
                Paginacion = await Paginacion<Perdidas>.CrearPaginacion(perdidas.AsNoTracking(), numeroPagina ?? 1, cantidadDeRegistros)
            };
            return (PartialView("_ListarPerdidas", modelo));
        }


        public async Task<IActionResult> IndexMisPerdidas()
        {
            ViewBag.ActiveView = "Publicaciones";
            ViewBag.SubView = "MisPerdidas";
            var perdidas = from perdida in _context.Perdidas select perdida;

            perdidas = perdidas.Where(a => a.Email == User.FindFirstValue("Email") && a.Baja == 0).OrderBy(a => a.Estado).ThenByDescending(a => a.Id).ThenBy(a => a.Nombre);

            return (View("IndexMisPerdidas", await perdidas.ToListAsync()));
        }

        [HttpGet]
        public async Task<IActionResult> ListarMisPerdidas()
        {
            var perdidas = from perdida in _context.Perdidas select perdida;

            perdidas = perdidas.Where(a => a.Email == User.FindFirstValue("Email") && a.Baja == 0).OrderBy(a => a.Estado).ThenByDescending(a => a.Id).ThenBy(a => a.Nombre);

            return (PartialView("_ListarMisPerdidas", await perdidas.ToListAsync()));
        }

        [HttpPost]
        public async Task<IActionResult> Recuperar(int id)
        {
            Perdidas perdida;
            perdida = _context.Perdidas.Where(a => a.Id == id).First();
            if (perdida != null)
            {
                perdida.Estado = 1;
                _context.Update(perdida);
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }
        public JsonResult ContactarPublicador(string remitente, string nombrePerro, string contenido, string destinatario, int perdidaId)
        {
            if (contenido == null)
            {
                return Json(new { error = true, message = "Por favor escriba algun mensaje" });
            }

            if (remitente != null)
            {
                ContactoPerdidas contactoPerdidas = _context.ContactoPerdidas.Where(ca => ca.EmailRemitente == remitente && ca.IdPerdida == perdidaId).FirstOrDefault();
                if (contactoPerdidas != null)
                {
                    return (Json(new { success = false, message = "Ya contactaste al dueño de esta publicación" }));
                }
                ContactoPerdidas contactoPerdidaNew = new ContactoPerdidas();
                contactoPerdidaNew.IdPerdida = perdidaId;
                contactoPerdidaNew.EmailRemitente = remitente;
                _context.ContactoPerdidas.Add(contactoPerdidaNew);
                _context.SaveChanges();
            }

            _ = EnviarCorreo(remitente, nombrePerro, contenido, destinatario);

            return (Json(new { success = true, message = "El correo fue enviado al dueño de la publicación con éxito!" }));
        }

        public async Task<IActionResult> Editar(Perdidas perdidaUpdate)
        {

            Perdidas perdida = _context.Perdidas.Where(a => a.Id == perdidaUpdate.Id).FirstOrDefault();
            if (validarCampos(perdidaUpdate))
            {
                return Json(new { error = true, perdida = perdida, mensaje = "Por favor complete todos los campos" });
            }
            if (perdida != null)
            {
                if (perdida.Nombre == perdidaUpdate.Nombre)
                {
                    perdida.Foto = perdidaUpdate.Foto;
                    perdida.Color = perdidaUpdate.Color;
                    perdida.Sexo = perdidaUpdate.Sexo;
                    perdida.Raza = perdidaUpdate.Raza;
                    perdida.Descripcion = perdidaUpdate.Descripcion;
                    perdida.FechaPerdida = perdidaUpdate.FechaPerdida;
                    perdida.Peso = perdidaUpdate.Peso;
                    _context.SaveChanges();
                    return (Json(new { success = true, perdida = perdida }));
                }
                else
                {
                    if (!hayAdopcionConEmailYNombre(perdidaUpdate.Email, perdidaUpdate.Nombre.ToLower()))
                    {
                        perdida.Foto = perdidaUpdate.Foto;
                        perdida.Color = perdidaUpdate.Color;
                        perdida.Sexo = perdidaUpdate.Sexo;
                        perdida.Raza = perdidaUpdate.Raza;
                        perdida.Descripcion = perdidaUpdate.Descripcion;
                        perdida.FechaPerdida = perdidaUpdate.FechaPerdida;
                        perdida.Peso = perdidaUpdate.Peso;
                        perdida.Nombre = perdidaUpdate.Nombre;
                        _context.SaveChanges();
                        return (Json(new { success = true, perdida = perdida }));

                    }
                }
                return Json(new { error = true, perdida = perdida, mensaje = "Ya existe una publicacion con este email y nombre de perro, por favor elija otro" });
            }
            return Json(new { error = true, mensaje = "Problema con la conexion a la base de datos" });

        }

        public async Task<IActionResult> BajaLogica(int id, string destinatario, string nombrePerro, string contenido)
        {
            Perdidas perdida;
            perdida = _context.Perdidas.Where(a => a.Id == id).First();
            if (perdida != null)
            {
                perdida.Baja = 1;
                _context.Update(perdida);
                await _context.SaveChangesAsync();
                if (User.IsInRole("Administrador"))
                {
                    _ = EnviarCorreoEliminarPerdida(nombrePerro, destinatario, contenido);
                }
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Obtener(int id)
        {
            // Obtener los datos de la adopción desde la base de datos
            Perdidas perdida = _context.Perdidas.FirstOrDefault(a => a.Id == id);

            if (perdida == null)
            {
                return NotFound(); // Adopción no encontrada
            }

            // Devolver los datos de la adopción en formato JSON
            return Json(new
            {
                id = perdida.Id,
                nombre = perdida.Nombre,
                raza = perdida.Raza,
                sexo = perdida.Sexo,
                foto = perdida.Foto,
                color = perdida.Color,
                descripcion = perdida.Descripcion,
                fechaPerdida = perdida.FechaPerdida,
                peso = perdida.Peso
            });
        }

        public async Task EnviarCorreo(string remitente, string nombrePerro, string contenido, string destinatario)
        {
            await Task.Run(() =>
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("", "ohmydoglem@gmail.com")); // Correo de origen, tiene que estar configurado en el metodo client.Authenticate()
                    message.To.Add(new MailboxAddress("", destinatario)); // Correo de destino
                    message.Subject = "Contacto de perdida para " + nombrePerro.ToUpper();
                    contenido = contenido + "<br>" + "<br>" + "<br>" + "El email de la persona que se contactó con usted es: " + remitente;
                    var bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = contenido;
                    message.Body = bodyBuilder.ToMessageBody();

                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        client.Connect("sandbox.smtp.mailtrap.io", 587, false);
                        client.Authenticate("d57c3b71f0c9fd", "40cce72119e038");
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

        public async Task EnviarCorreoEliminarPerdida(string nombrePerro, string destinatario, string mensaje)
        {
            await Task.Run(() =>
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("", "ohmydoglem@gmail.com")); // Correo de origen, tiene que estar configurado en el metodo client.Authenticate()
                    message.To.Add(new MailboxAddress("", destinatario)); // Correo de destino
                    message.Subject = "Baja de la publicacion de la perdida de " + nombrePerro.ToUpper();
                    string contenido = "La veterinaria OhMyDog se pone en contacto con usted para notificarle que la publicacion de perdida de " + nombrePerro.ToUpper() + " fue dada de baja, " 
                            + "el motivo es el siguiente: \"" + mensaje + "\". "
                            + "Cualquier duda envie un mail a ohmydog@gmail.com";
                    var bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = contenido;
                    message.Body = bodyBuilder.ToMessageBody();

                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        client.Connect("sandbox.smtp.mailtrap.io", 587, false);
                        client.Authenticate("d57c3b71f0c9fd", "40cce72119e038");
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

        public bool hayAdopcionConEmailYNombre(string email, string nombre)
        {
            Perdidas perdida = _context.Perdidas.Where(a => a.Email == email && a.Nombre == nombre && a.Estado == 0).FirstOrDefault();
            if (perdida != null)
            {
                if (perdida.Baja == 1)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool validarCampos(Perdidas perdida)
        {
            if (perdida.Nombre == null || perdida.Peso == null || perdida.Color == null || perdida.Raza == null || perdida.Sexo == null || perdida.Descripcion == null)
            {
                return true;
            }
            return false;
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
