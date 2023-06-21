using Aplicacion.Data;
using Aplicacion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Security.Claims;

namespace Aplicacion.Controllers
{
    public class AdopcionesController : Controller
    {
        private readonly OhmydogdbContext _context;

        public AdopcionesController(OhmydogdbContext context)
        {
            _context = context;
        }
        private int cantidadDeRegistros = 10;
        public IActionResult PublicarAdopcionesIndex()
        {
            //ViewBag.ActiveView = "PublicarAdopcion";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PublicarAdopcionesIndex(Adopciones adopcion)
        {

            if (hayAdopcionConEmailYNombre(adopcion.Email, adopcion.Nombre.ToLower()))
            {
                ViewBag.Message = "Adopcion fallida. Ya existe una adopcion para este usuario con ese nombre de perro, por favor elija otro nombre";
                return View();
            }
            adopcion.Nombre = adopcion.Nombre.ToLower();
            _context.Add(adopcion);
            await _context.SaveChangesAsync();

            return RedirectToAction("IndexMisAdopciones", "Adopciones");
        }

        [HttpGet]
        public async Task<IActionResult> IndexAdopciones(string query, int? numeroPagina)
        {
            var adopciones = from adopcion in _context.Adopciones select adopcion;

            if (query != null && numeroPagina == null)
            {
                numeroPagina = 1;
            }
            if (!String.IsNullOrEmpty(query))
            {
                adopciones = adopciones.Where(a => a.Nombre.Contains(query));
            }

            adopciones = adopciones.Where(a => a.Email != User.FindFirstValue("Email") && a.Baja == 0).OrderBy(a => a.Estado).ThenByDescending(a => a.Id).ThenBy(a => a.Nombre);

            AdopcionViewModel modelo = new AdopcionViewModel
            {
                Origen = (query != null) ? true : false,
                Paginacion = await Paginacion<Adopciones>.CrearPaginacion(adopciones.AsNoTracking(), numeroPagina ?? 1, cantidadDeRegistros)
            };
            return (PartialView("_ListarAdopciones", modelo));
        }

        [HttpGet]
        public async Task<IActionResult> IndexMisAdopciones(string query, int? numeroPagina)
        {
            var adopciones = from adopcion in _context.Adopciones select adopcion;

            if (query != null && numeroPagina == null)
            {
                numeroPagina = 1;
            }
            if (!String.IsNullOrEmpty(query))
            {
                adopciones = adopciones.Where(a => a.Nombre.Contains(query));
            }

            adopciones = adopciones.Where(a => a.Email == User.FindFirstValue("Email") && a.Baja == 0).OrderBy(a => a.Estado).ThenByDescending(a => a.Id).ThenBy(a => a.Nombre);

            AdopcionViewModel modelo = new AdopcionViewModel
            {
                Origen = (query != null) ? true : false,
                Paginacion = await Paginacion<Adopciones>.CrearPaginacion(adopciones.AsNoTracking(), numeroPagina ?? 1, cantidadDeRegistros)
            };
            return (PartialView("_ListarMisAdopciones", modelo));
        }

        [HttpPost]
        public async Task<IActionResult> Adoptar(int id)
        {
            Adopciones adopcion;
            adopcion = _context.Adopciones.Where(a => a.Id == id).First();
            if (adopcion != null)
            {
                adopcion.Estado = 1;
                _context.Update(adopcion);
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }
        public JsonResult ContactarPublicador(string remitente, string remitenteNombre, string nombrePerro, string contenido, string destinatario)
        {
            _ = EnviarCorreo(remitente, remitenteNombre, nombrePerro, contenido, destinatario);

            return (Json(new { success = true, message = "El correo fue enviado al paseador con éxito!" }));
        }

        public async Task<IActionResult> Editar(Adopciones adopcionUpdate)
        {
           
            Adopciones adopcion = _context.Adopciones.Where(a => a.Id == adopcionUpdate.Id).FirstOrDefault();
            if (validarCampos(adopcionUpdate))
            {
                return Json(new { error = true, adopcion = adopcion, mensaje = "Por favor complete todos los campos" });
            }
            if (adopcion != null)
            {
                if (adopcion.Nombre == adopcionUpdate.Nombre)
                {
                    adopcion.Tamano = adopcionUpdate.Tamano;
                    adopcion.Color = adopcionUpdate.Color;
                    adopcion.Sexo = adopcionUpdate.Sexo;
                    adopcion.Raza = adopcionUpdate.Raza;
                    adopcion.Descripcion = adopcionUpdate.Descripcion;
                    _context.SaveChanges();
                    return (Json(new { success = true, adopcion = adopcion }));
                }
                else
                {
                    if (!hayAdopcionConEmailYNombre(adopcionUpdate.Email, adopcionUpdate.Nombre.ToLower()))
                    {
                        adopcion.Nombre = adopcionUpdate.Nombre;
                        adopcion.Tamano = adopcionUpdate.Tamano;
                        adopcion.Color = adopcionUpdate.Color;
                        adopcion.Sexo = adopcionUpdate.Sexo;
                        adopcion.Raza = adopcionUpdate.Raza;
                        adopcion.Descripcion = adopcionUpdate.Descripcion;
                        _context.SaveChanges();
                        return (Json(new { success = true, adopcion = adopcion}));

                    }
                }              
                return Json(new { error = true, adopcion = adopcion, mensaje = "Ya existe una publicacion con ese email y nombre por favor elija otro" });   
            }
            return Json(new { error = true, mensaje = "Problema con la conexion a la base de datos" });

        }

        public async Task<IActionResult> BajaLogica(int id)
        {
            Adopciones adopcion;
            adopcion = _context.Adopciones.Where(a => a.Id == id).First();
            if (adopcion != null)
            {
                adopcion.Baja = 1;
                _context.Update(adopcion);
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Obtener(int id)
        {
            // Obtener los datos de la adopción desde la base de datos
            Adopciones adopcion = _context.Adopciones.FirstOrDefault(a => a.Id == id);

            if (adopcion == null)
            {
                return NotFound(); // Adopción no encontrada
            }

            // Devolver los datos de la adopción en formato JSON
            return Json(new
            {
                id = adopcion.Id,
                nombre = adopcion.Nombre,
                raza = adopcion.Raza,
                sexo = adopcion.Sexo,
                tamano = adopcion.Tamano,
                color = adopcion.Color,
                descripcion = adopcion.Descripcion
            });
        }

        public async Task EnviarCorreo(string remitente, string remitenteNombre, string nombrePerro, string contenido, string destinatario)
        {
            await Task.Run(() =>
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("", "ohmydoglem@gmail.com")); // Correo de origen, tiene que estar configurado en el metodo client.Authenticate()
                    message.To.Add(new MailboxAddress("", destinatario)); // Correo de destino
                    message.Subject = "Contacto de " + remitenteNombre + " por la adopcion de " + nombrePerro;
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

        public bool hayAdopcionConEmailYNombre(string email, string nombre)
        {
            Adopciones adopcion = _context.Adopciones.Where(a => a.Email == email && a.Nombre == nombre && a.Estado == 0).FirstOrDefault();
            if (adopcion != null)
            {
                if (adopcion.Baja == 1)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool validarCampos(Adopciones adopcion)
        {
            if (adopcion.Nombre == null || adopcion.Tamano == null || adopcion.Color == null || adopcion.Raza == null || adopcion.Sexo == null || adopcion.Descripcion == null)
            {
                return true;
            }
            return false;
        }
    }
}
