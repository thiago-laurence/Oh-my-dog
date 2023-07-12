using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Models;
using Aplicacion.Data;
using static System.Net.Mime.MediaTypeNames;
using MimeKit;
using System.Text;
using Hangfire;

namespace Aplicacion.Controllers
{
    public class PerrosController : Controller
    {
        private readonly OhmydogdbContext _context;

        public PerrosController(OhmydogdbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int idDueno)
        {
            return _context.Perros != null ?
                          View(_context.Usuarios.Where(u => u.Id == idDueno).Include(u => u.GetPerros.OrderByDescending(p => p.Estado).ThenBy(p => p.Nombre)).First()) :
                          Problem("Entity set 'OhmydogdbContext.Perros'  is null.");
        }

        public async Task<IActionResult> HistoriaClinica(int idPerro)
        {
            var tratamientos = await _context.TratamientoPerros.Include(t => t.IdTratamientoNavigation).Where(t => t.IdPerro == idPerro)
                                .OrderBy(t => t.Fecha).ToListAsync();
            var perro = await _context.Perros.Where(p => p.Id == idPerro).FirstOrDefaultAsync();
            perro!.TratamientoPerros = tratamientos;
            return (
                    (_context.Perros != null) ? View("HistoriaClinica", new TratamientosViewModel
                    {
                        Perro = perro,
                        Tratamientos = await _context.Tratamientos.ToListAsync()
                    }) 
                    : Problem("Entity set 'OhmydogdbContext.Perros'  is null.")
                );
        }

        public async Task<IActionResult> CalendarioDeVacunas(int idPerro)
        {
            var vacunas = await _context.VacunaPerros.Include(v => v.IdVacunaNavigation).Where(v => v.IdPerro == idPerro)
                                .OrderBy(v => v.FechaAplicacion).ToListAsync();
            var perro = await _context.Perros.Where(p => p.Id == idPerro).FirstOrDefaultAsync();
            perro!.VacunaPerros = vacunas;
            return (
                    (_context.Perros != null) ? View("CalendarioDeVacunas", new VacunasViewModel
                    {
                        Perro = perro,
                        Vacunas = await _context.Vacunas.ToListAsync()
                    })
                    : Problem("Entity set 'OhmydogdbContext.Perros'  is null.")
                );
        }

        [HttpPost]
        public async Task<JsonResult> RegistrarPerro([FromBody] Perros perro)
        {
            if (ExistePerro(perro.Id, perro.Nombre, perro.IdDueno))
            {
                return (Json(new { success = false, message = "El nombre del perro ya está registrado para un perro activo al cliente asociado" } ));
            }
            perro.Estado = true;

            await _context.Perros.AddAsync(perro);
            await _context.SaveChangesAsync();

            return (Json(new { success = true, message = "¡El perro se ha registrado con éxito!" }));
        }

        [HttpPost]
        public JsonResult RegistrarTratamiento(TratamientoPerro tratamiento)
        {
            _context.TratamientoPerros.Add(tratamiento);
            _context.SaveChanges();

            return (Json(new { success = true, message = "¡El tratamiento se ha registrado con éxito!" }));
        }

        [HttpPost]
        public JsonResult RegistrarVacunacion(VacunaPerro vacunacion)
        {
            _context.VacunaPerros.Add(vacunacion);
            _context.SaveChanges();

            return (Json(new { success = true, message = "¡La vacuna se ha registrado con éxito!" }));
        }

        [HttpGet]
        public async Task<IActionResult> ListarPerros(int idDueno)
        {
            return (PartialView("_ListarPerros", await _context.Usuarios.Include(u => u.GetPerros.OrderByDescending(p => p.Estado).ThenBy(p => p.Nombre)).FirstOrDefaultAsync(u => u.Id == idDueno)));
        }

        [HttpGet]
        public async Task<IActionResult> ListarTratamientos(int idPerro)
        {
            var tratamientos = await _context.TratamientoPerros.Include(t => t.IdTratamientoNavigation).Where(t => t.IdPerro == idPerro)
                                .OrderBy(t => t.Fecha).ToListAsync();
            var perro = await _context.Perros.Where(p => p.Id == idPerro).FirstOrDefaultAsync();
            perro!.TratamientoPerros = tratamientos;
            return (PartialView("_ListarTratamientos", perro));
        }

        [HttpGet]
        public async Task<IActionResult> ListarVacunas(int idPerro)
        {
            var vacunas = await _context.VacunaPerros.Include(v => v.IdVacunaNavigation).Where(v => v.IdPerro == idPerro)
                                .OrderBy(v => v.FechaAplicacion).ToListAsync();
            var perro = await _context.Perros.Where(p => p.Id == idPerro).FirstOrDefaultAsync();
            perro!.VacunaPerros = vacunas;
            return (PartialView("_ListarVacunas", perro));
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

        public bool ExistePerro(int idPerro, string nombrePerro, int idDueno)
        {
            return (
                    _context.Perros.Any(p => (p.IdDueno == idDueno) && (p.Id != idPerro) && (p.Estado == true) && (p.Nombre == nombrePerro))
                   );
        }

        [HttpPost]
        public async Task<JsonResult> ObtenerPerro(int id)
        {
            return(Json(await _context.Perros.FirstOrDefaultAsync(m => m.Id == id)));
        }

        [HttpPost]
        public JsonResult ModificarPerro(Perros perro)
        {
            var _perro = _context.Perros.FirstOrDefault(p => p.Id == perro.Id);
            if (ExistePerro(perro.Id, perro.Nombre, perro.IdDueno))
            {
                return (Json(new { success = false, message = "El nombre del perro ya está registrado para un perro activo al cliente asociado" }));
            }

            if (!perro.Estado)
            {
                var publicacion = _context.PublicacionTinderdog.Where(p => p.IdPerro == perro.Id).Include(p => p.NoMeGustaRecibidos).Include(p => p.MeGustaRecibidos).FirstOrDefault();
                if (publicacion != null){
                    _context.PerrosMeGusta.RemoveRange(publicacion.MeGustaRecibidos);
                    _context.PerrosNoMeGusta.RemoveRange(publicacion.NoMeGustaRecibidos);
                    _context.PublicacionTinderdog.Remove(publicacion);
                    _context.SaveChanges();
                }
            }

            _context.Perros.Remove(_perro);
            _context.Perros.Add(perro);
            _context.SaveChanges();

            return (Json(new { success = true, message = "¡El perro ha sido modificado con éxito!" }));
        }

        [HttpGet]
        public JsonResult ExisteCastracionAplicada(int idPerro, int idTratamiento)
        {
            var idCastracion = _context.Tratamientos.FirstOrDefault(t => t.Nombre == "Castración")?.Id;
            if (idCastracion != idTratamiento)
            {
                return (Json(new { castracion = false, message = "" }));
            }

            var _okCastracion = _context.TratamientoPerros.Any(t => (t.IdPerro == idPerro) && (t.IdTratamiento == idCastracion));
            if (_okCastracion)
            {
                return (Json(new { castracion = true, message = "Ya existe un tratamiento de tipo \"Castración\" realizado al perro" }));
            }
            return (Json(new { castracion = false, message = "" }));
        }

        [HttpGet]
        public JsonResult ValidarVacuna(int idPerro, int idVacuna)
        {
            var idAntirrabica = _context.Vacunas.FirstOrDefault(t => t.Nombre == "Antirrábica")?.Id;
            if (idVacuna == idAntirrabica)
            {
                return (ExisteAntirrabicaAplicada(idPerro, idVacuna));
            }

            var idMoquillo = _context.Vacunas.FirstOrDefault(t => t.Nombre == "Moquillo")?.Id;
            if (idVacuna == idMoquillo)
            {
                return (ExisteMoquilloAplicada(idPerro, idVacuna));
            }

            return (Json(new { success = true, message = "" }));
        }

        [HttpGet]
        public JsonResult ExisteAntirrabicaAplicada(int idPerro, int idVacuna)
        {
            var _existeAntirrabica = _context.VacunaPerros.Any(t => (t.IdPerro == idPerro) && (t.IdVacuna == idVacuna));
            if (!_existeAntirrabica) // Si NO aplico la vacuna antirrabica nunca, se valida que el perro posea mas de 4 meses de edad
            {
                var _perro = _context.Perros.FirstOrDefault(p => p.Id == idPerro);
                if ((DateTime.Today - _perro?.FechaDeNacimiento)?.TotalDays < 120)
                {
                    return (Json(new { success = false, message = "La vacuna de tipo \"Antirrábica\" debe aplicarse luego de los 4 meses de edad" }));
                }
                return (Json(new { success = true, message = "" }));
            }

            // Si ya se aplico la vacuna antirrabica, se valida que la nueva aplicacion este dentro del periodo de 1 año desde la ultima aplicacion (la validacion no es restrictiva, sino informativa)
            var _antirrabica = _context.VacunaPerros
                                .Where(t => t.IdPerro == idPerro && t.IdVacuna == idVacuna)
                                .OrderByDescending(t => t.FechaAplicacion)
                                .LastOrDefault();
            if ((DateTime.Today - _antirrabica?.FechaAplicacion)?.TotalDays < 365)
            {
                return (Json(new { success = true, message = "Aún no ha transcurrido el año desde la última aplicación de la vacuna \"Antirrábica\", se recomienda " +
                    "que al menos haya pasado ese tiempo" }));
            }

            return (Json(new { success = true, message = "" }));
        }

        [HttpGet]
        public JsonResult ExisteMoquilloAplicada(int idPerro, int idVacuna)
        {
            var _existeMoquillo = _context.VacunaPerros.Any(t => (t.IdPerro == idPerro) && (t.IdVacuna == idVacuna));
            if (!_existeMoquillo) // Si NO aplico la vacuna moquillo nunca, se valida que sea despues de los 2 meses de edad
            {
                var _perro = _context.Perros.FirstOrDefault(p => p.Id == idPerro);
                if ((DateTime.Today - _perro?.FechaDeNacimiento)?.TotalDays < 60)
                {
                    return (Json(new { success = false, message = "La vacuna de tipo \"Moquillo\" debe aplicarse luego de los 2 meses de edad" }));
                }
                return (Json(new { success = true, message = "" }));
            }

            // Si ya se aplico la vacuna moquillo, se valida que la nueva aplicacion sea luego de 21 dias de la primera aplicacion (la validacion no es restrictiva, sino informativa)
            var _moquillo = _context.VacunaPerros
                                .Where(t => t.IdPerro == idPerro && t.IdVacuna == idVacuna)
                                .OrderBy(t => t.FechaAplicacion)
                                .ToList();
            VacunaPerro _vacuna;
            if(_moquillo.Count() == 1) // Si existe una unica aplicacion, advierto por los 21 dias de la segunda aplicacion
            {
                _vacuna = _moquillo.First();
                if ((DateTime.Today - _vacuna?.FechaAplicacion)?.TotalDays < 21)
                {
                    return (Json(new
                    {
                        success = true,
                        message = "Aún no han trascurrido 21 dias desde la última aplicación de la vacuna \"Moquillo\", se recomienda " +
                        "que al menos haya trascurrido ese tiempo para la aplicación del refuerzo"
                    }));
                }
            }

            // Si existen mas apliacaciones, obtengo la ultima de ellas y se valida que la nueva aplicacion este dentro del periodo de 1 año desde la ultima aplicacion.
            _vacuna = _moquillo.Last();
            if ((DateTime.Today - _vacuna?.FechaAplicacion)?.TotalDays < 365)
            {
                return (Json(new
                {
                    success = true,
                    message = "Aún no ha transcurrido el año desde la última aplicación de la vacuna \"Moquillo\", se recomienda " +
                    " que al menos haya pasado ese tiempo"
                }));
            }

            return (Json(new { success = true, message = "" }));
        }

        [HttpPost]
        public JsonResult ProgramarRecordatorio(DateTime fechaRecordatorio, string idDueno, int idVacuna, int idPerro)
        {
           //DateTime fechaHardcoding = new DateTime(2023, 06, 23, 11, 15, 0);
           //fechaHardcoding = fechaHardcoding.AddMinutes(1);

            var query = _context.Perros.Where(p => p.Id == idPerro)
                .Join(_context.VacunaPerros, p => p.Id, vp => vp.IdPerro, (p, vp) => new { Perro = p.Nombre, Dueno = p.IdDueno, Vacuna = vp.IdVacuna })
                .Where(obj => obj.Vacuna == idVacuna)
                .Join(_context.Vacunas, obj => obj.Vacuna, v => v.Id, (obj, v) => new { Perro = obj.Perro, Dueno = obj.Dueno, Vacuna = v.Nombre })
                .Join(_context.Usuarios, obj => obj.Dueno, u => u.Id, (obj, u) => new { Perro = obj.Perro, Dueno = (u.Nombre + " " + u.Apellido), Email = u.Email, Vacuna = obj.Vacuna });
            var obj = query.First();

            string remitente = "ohmydog@gmail.com",
                destinatario = obj.Email,
                asunto = "Recordatorio de vacunación",
                contenido = "Hola " + obj.Dueno + ",  este mensaje es para informar que el dia " + fechaRecordatorio.ToShortDateString() 
                + " se vence la vacuna " + obj.Vacuna + " para su perro " + obj.Perro + ", por lo que se le recomienda realizar una solicitud para un nuevo turno.<br>" +
                "Saludos del equipo de <strong>OhMyDog!</strong>";

            // Programar el método para ejecutarse en la fecha y hora específica utilizando Hangfire            
            BackgroundJob.Schedule(() => EnviarRecordatorio(remitente, asunto, contenido, destinatario), fechaRecordatorio);
            //BackgroundJob.Schedule(() => EnviarRecordatorio(remitente, asunto, contenido, destinatario), fechaHardcoding);

            return (Json(new { success = true }));
        }

        public async Task EnviarRecordatorio(string remitente, string asunto, string contenido, string destinatario)
        {
            await Task.Run(() =>
            {
                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("", "ohmydoglem@gmail.com")); // Correo de origen, tiene que estar configurado en el metodo client.Authenticate()
                    message.To.Add(new MailboxAddress("", destinatario)); // Correo de destino
                    message.Subject = asunto;
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
    }
}
