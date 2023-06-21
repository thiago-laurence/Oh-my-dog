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
using Hangfire;

namespace Aplicacion.Controllers
{
    public class TurnosController : Controller
    {
        private readonly OhmydogdbContext _context;

        public TurnosController(OhmydogdbContext context)
        {
            _context = context;
        }

        // GET: Turnos
        public async Task<IActionResult> Index()
        {
            var ohmydogdbContext = _context.Turnos.Include(t => t.EstadoNavigation);
            return View(await ohmydogdbContext.ToListAsync());
        }

        // GET: Turnos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Turnos == null)
            {
                return NotFound();
            }

            var turnos = await _context.Turnos
                .Include(t => t.EstadoNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (turnos == null)
            {
                return NotFound();
            }

            return View(turnos);
        }


		public String obtenerPerros(String mail)
		{
			 int idDueno = _context.Usuarios.Where(m => m.Email == mail).Select(i => i.Id).First();
            var perrosUsuario = _context.Perros.Where(c => c.IdDueno == idDueno && c.Estado==true).ToList();
            Console.Write(perrosUsuario.Count());
			return JsonSerializer.Serialize(perrosUsuario);
		}

        public IActionResult obtenerEventos(String mail)
		{
			int idDueno = _context.Usuarios.Where(m => m.Email == mail).Select(i => i.Id).First();
			int? rol = _context.Usuarios.Where(m => m.Email == mail).Select(i => i.IdRol).First();
			DateTime fechaActual = DateTime.Now.Date;
            //Si quedaron pendientes los cancelo
            List<Turnos> turnosActualizar=_context.Turnos.Where(i => i.Fecha < fechaActual && i.Estado==3).ToList();
            turnosActualizar.ForEach(turnoBorrar => {
                turnoBorrar.Estado = 2;
			});
            borrarMañana();
            borrarTarde();
            
            _context.SaveChanges();

			if (rol == 1) {

                var turnosAux = _context.Turnos
    .Select(t => new
    {
        Id = t.Id,
        Horario= _context.HorarioTurnos.Where(i=>i.Id==t.Horario).Select(p=>p.Turno).First(),
        Dueno = t.Dueno,
        Motivo= t.Motivo,
        Estado= t.Estado,
        Fecha= t.Fecha,
		Cliente = _context.Usuarios.Where(i => t.Dueno == i.Id).Select(m=>m.Email).First(),
		Comentario= t.Comentario,
        HorarioFinal= t.HorarioFinal,
		PerrosDelTurno = _context.PerroTurnos.Where(pt => pt.IdTurno == t.Id).Select(pt => new {
        Nombre =pt.Nombre,
        }).ToList()
    })
	.ToList();

				
				return Json(new { admin = true, turnos= JsonSerializer.Serialize(turnosAux) });
			}
            else
            {

				var turnoAux = _context.Turnos.Where(t => idDueno == t.Dueno)
	.Select(t => new
	{
		Id = t.Id,
		Horario = _context.HorarioTurnos.Where(i => i.Id == t.Horario).Select(p => p.Turno).First(),
		Dueno = t.Dueno,
		Motivo = t.Motivo,
		Estado = t.Estado,
		Fecha = t.Fecha,
		Comentario = t.Comentario,
		HorarioFinal = t.HorarioFinal,
		Cliente = _context.Usuarios.Where(i => t.Dueno == i.Id).Select(m => m.Email).First(),
		PerrosDelTurno = _context.PerroTurnos.Where(pt => pt.IdTurno == t.Id).Select(p=>new
        {

         Nombre=   p.Nombre,
        }).ToList()
	})
	.ToList();


				return Json(new { admin = false, turnos = JsonSerializer.Serialize(turnoAux) });
			}
			
		}


        [HttpPost]
        public IActionResult cargarTurno(String mail,DateTime fecha, List<String> perros, String motivo,int horario)
        {
			Turnos turno = new Turnos();
			turno.Estado = 3;
			turno.Motivo = motivo;
			turno.Dueno = _context.Usuarios.Where(m => m.Email == mail).Select(i => i.Id).First();
			turno.Fecha = fecha;
			turno.Horario = (horario == 0) ? 1 : 2;
            if (_context.Turnos.Where(i => i.Fecha == fecha.Date && i.Dueno == turno.Dueno).Count() == 0) { 
            _context.Turnos.Add(turno);
			_context.SaveChanges();
			int id_Turno = _context.Turnos.Max(i=>i.Id); // Retrieve the generated ID from the newly inserted Turnos record

			perros.ForEach(perro => {
				PerroTurnos turnoPerro = new PerroTurnos();
				turnoPerro.Nombre = perro;
				turnoPerro.IdTurno = id_Turno;
                int? idPerro = _context.Perros
                    .Where(m => m.IdDueno == turno.Dueno && m.Nombre == perro)?
                    .Select(i => i.Id)
                    .FirstOrDefault();
                if (idPerro == 0) { 
                turnoPerro.IdPerro = null; // Retrieve the ID of the Perro record
                }
                else
                {
					turnoPerro.IdPerro = idPerro;
				}

				_context.PerroTurnos.Add(turnoPerro);
				
			});
			_context.SaveChanges();
            var turnoAux = _context.Turnos
     .Select(t => new
     {
         Id = t.Id,
         Horario = _context.HorarioTurnos.Where(i => i.Id == t.Horario).Select(p => p.Turno).First(),
         Dueno = t.Dueno,
         Motivo = t.Motivo,
         Estado = t.Estado,
         Fecha = t.Fecha,
         Cliente = mail,
	

		 PerrosDelTurno = _context.PerroTurnos.Where(pt => pt.IdTurno == t.Id).Select(pt => new
         {
             Nombre = pt.Nombre,
         }).ToList()
        }).OrderBy(i=>i.Id).Last();
		return Json(new{asignado= JsonSerializer.Serialize(turnoAux), obtenido=true});
			}else
            {
                return Json(new { obtenido = false });
            }
		}

		// GET: Turnos/Create
		public IActionResult Create()
        {
            ViewData["Estado"] = new SelectList(_context.EstadoTurnos, "Id", "Id");
            return View();
        }

        // POST: Turnos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Motivo,Estado,Fecha,Dueno")] Turnos turnos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(turnos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Estado"] = new SelectList(_context.EstadoTurnos, "Id", "Id", turnos.Estado);
            return View(turnos);
        }

        // GET: Turnos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Turnos == null)
            {
                return NotFound();
            }

            var turnos = await _context.Turnos.FindAsync(id);
            if (turnos == null)
            {
                return NotFound();
            }
            ViewData["Estado"] = new SelectList(_context.EstadoTurnos, "Id", "Id", turnos.Estado);
            return View(turnos);
        }

        // POST: Turnos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Motivo,Estado,Fecha,Dueno")] Turnos turnos)
        {
            if (id != turnos.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(turnos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TurnosExists(turnos.Id))
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
            ViewData["Estado"] = new SelectList(_context.EstadoTurnos, "Id", "Id", turnos.Estado);
            return View(turnos);
        }

        // GET: Turnos/Delete/5
        [HttpPost]
        public IActionResult rechazarTurno(int idTurno, String comentario, String horario)
        {

			Turnos turno = _context.Turnos.Where(m => m.Id == idTurno).First();
			turno.Estado = 2;
			turno.Comentario = comentario;
			turno.HorarioFinal = horario;
			Turnos borrado = _context.Turnos.Where(m => m.Id == idTurno).First();
			List<PerroTurnos> turnosPerrosBorrar = _context.PerroTurnos.Where(m => m.IdTurno == idTurno).ToList();
			List<PerroTurnos> copia = new List<PerroTurnos>();
			turnosPerrosBorrar.ForEach(turno =>
			{
                copia.Add(turno);
				_context.PerroTurnos.Remove(turno);
			});
			_context.SaveChanges();
			_context.Turnos.Remove(borrado);
			_context.SaveChanges();
			turno.Id = 0;
			
			_context.Turnos.Add(turno);
			_context.SaveChanges();
			int turnoFinal = _context.Turnos.Max(i => i.Id);
			copia.ForEach(turno =>
			{
                turno.IdTurno = turnoFinal;
				turno.Id = 0;
				_context.PerroTurnos.Add(turno);
			});
			_context.SaveChanges();

			var turnoAux = _context.Turnos
	.Select(t => new
	{
		Id = t.Id,
		Horario = _context.HorarioTurnos.Where(i => i.Id == t.Horario).Select(p => p.Turno).First(),
		Dueno = t.Dueno,
		Motivo = t.Motivo,
		Estado = t.Estado,
		Fecha = t.Fecha,
		Cliente = _context.Usuarios.Where(i => t.Dueno == i.Id).Select(m => m.Email).First(),
		Comentario = t.Comentario,
		HorarioFinal = t.HorarioFinal,
		PerrosDelTurno = _context.PerroTurnos.Where(pt => pt.IdTurno == t.Id).Select(pt => new {
			Nombre = pt.Nombre,
		}).ToList()
	}).OrderBy(i=>i.Id).Last();
			EnviarCorreo(turnoAux,false);
			return Json(new { turno = JsonSerializer.Serialize(turnoAux) });
        }



		[HttpPost]
		public IActionResult borrarMañana()
        {
			DateTime fechaActual = DateTime.Now.Date;
            //Si quedaron pendientes los cancelo
            int horaMan = DateTime.Now.Hour;
			if (horaMan > 12) { 
				List<Turnos> turnosActualizar = _context.Turnos.Where(i => i.Fecha == fechaActual && i.Estado == 3 && i.Horario==1).ToList();
			turnosActualizar.ForEach(turnoBorrar => {
				turnoBorrar.Estado = 2;
                turnoBorrar.Comentario = "Cancelado por tiempo";
			});
				_context.SaveChanges();
			}
			
			return Json(true);

        }
        [HttpPost]
		public IActionResult borrarTarde()
		{
			DateTime fechaActual = DateTime.Now.Date;
            int horaTarde =  DateTime.Now.Hour;
			if (horaTarde> 19)
            //Si quedaron pendientes los cancelo
            {
                List<Turnos> turnosActualizar = _context.Turnos.Where(i => i.Fecha == fechaActual && i.Estado == 3 && i.Horario == 2).ToList();
                turnosActualizar.ForEach(turnoBorrar =>
                {
					turnoBorrar.Comentario = "Cancelado por tiempo";
					turnoBorrar.Estado = 2;
                });

                _context.SaveChanges();
            }
                return Json(true);

		}



		[HttpPost]
		public IActionResult aceptarTurno(int idTurno, String comentario, String horario)
		{
            
            Turnos turno = _context.Turnos.Where(m => m.Id == idTurno).First();
            turno.Estado = 1;
            turno.Comentario = comentario;
            turno.HorarioFinal = horario;
           
            Turnos borrado= _context.Turnos.Where(m => m.Id == idTurno).First();
			List<PerroTurnos> turnosPerrosBorrar = _context.PerroTurnos.Where(m => m.IdTurno == idTurno).ToList();
            List<PerroTurnos> copia = new List<PerroTurnos>();
			turnosPerrosBorrar.ForEach(turno =>
			{
                copia.Add(turno);
				_context.PerroTurnos.Remove(turno);
			});
			_context.SaveChanges();
			_context.Turnos.Remove(borrado);
			_context.SaveChanges();
			turno.Id = 0;
			_context.Turnos.Add(turno);
			_context.SaveChanges();
			int turnoFinal = _context.Turnos.Max(i => i.Id);
			copia.ForEach(turno =>
			{
				turno.IdTurno = turnoFinal;
                turno.Id = 0;
				_context.PerroTurnos.Add(turno);
			});
			_context.SaveChanges();
			var turnoAux = _context.Turnos
	.Select(t => new
	{
		Id = t.Id,
		Horario = _context.HorarioTurnos.Where(i => i.Id == t.Horario).Select(p => p.Turno).First(),
		Dueno = t.Dueno,
		Motivo = t.Motivo,
		Estado = 1,
		Fecha = t.Fecha,
		Cliente = _context.Usuarios.Where(i => t.Dueno == i.Id).Select(m => m.Email).First(),
		Comentario = t.Comentario,
		HorarioFinal = t.HorarioFinal,
		PerrosDelTurno = _context.PerroTurnos.Where(pt => pt.IdTurno == t.Id).Select(pt => new {
			Nombre = pt.Nombre,
		}).ToList()
	}).OrderBy(i => i.Id).Last();
            if (turnoAux.Fecha.AddDays(-3).Date > DateTime.Now.Date)
            {
                Console.WriteLine("Se agendo el recordatorio");
                BackgroundJob.Schedule(() => EnviarCorreo(turnoAux, true), turnoAux.Fecha.AddDays(-3).AddHours(11).AddMinutes(30));
            }
			EnviarCorreo(turnoAux,false);
			return Json(new { turno= JsonSerializer.Serialize(turnoAux)});
		}


		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Turnos == null)
            {
                return NotFound();
            }

            var turnos = await _context.Turnos
                .Include(t => t.EstadoNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (turnos == null)
            {
                return NotFound();
            }

            return View(turnos);
        }

        // POST: Turnos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Turnos == null)
            {
                return Problem("Entity set 'OhmydogdbContext.Turnos'  is null.");
            }
            var turnos = await _context.Turnos.FindAsync(id);
            if (turnos != null)
            {
                _context.Turnos.Remove(turnos);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }









		public async Task EnviarCorreo(dynamic turno,bool esRecordatorio)
		{
            await Task.Run(() =>
            {
				try
				{
					int id = turno.Id;
					int estado = turno.Estado;
					var horario = turno.Horario;
                    
					int dueno = turno.Dueno;
					var fecha = turno.Fecha;
					var horarioFinal = turno.HorarioFinal;
					var perroTurnos = turno.PerrosDelTurno;
					var comentario = turno.Comentario;
                    string cliente = turno.Cliente;
					var message = new MimeMessage();
					message.From.Add(new MailboxAddress("", "ohmydoglem@gmail.com")); // Correo de origen, tiene que estar configurado en el metodo client.Authenticate()
					message.To.Add(new MailboxAddress("", cliente)); // Correo de destino
                    var asunto="";
                    var contenido = "";
                    if (estado == 1) {

                        
						asunto = "Turno aceptado";
                        if (esRecordatorio)
                        {
                            contenido = "Se le recuerda ";
                        }
                        else
                        {
                            contenido = "Se le informa ";
                        }
                        contenido += "que su turno ha sido aceptado. A continuación el detalle de su turno: "+
						"<br>"+ "<br>"+"Fecha: "+fecha.ToString("dddd, dd MMMM yyyy") + "<br>"+"Horario del turno: "+horarioFinal+"<br>"+"Perros: "+"<br>"+"<ul>";
                        foreach(var turno in perroTurnos)
                        {
                            contenido+="<li>"+turno.Nombre+ "</li>";
                        }
                        contenido += "</ul>";
                        if (comentario != null)
                        {
                            contenido += "<br>" + comentario;
                        }
                    }
                    else
                    {
                        asunto = "Turno rechazado";
						
						contenido = "Se le informa que su turno ha sido rechazado. A continuación el detalle de su turno: " +
						"<br>" + "<br>" + "Fecha: " + fecha.ToString("dddd, dd MMMM yyyy") + "<br>" + "Franja solicitada: " + horario + "<br>" + "Perros: " + "<br>" + "<ul>";
						foreach (var turno in perroTurnos)
						{
							contenido += "<li>" + turno.Nombre + "</li>";
						}
						contenido += "</ul>";
						contenido += "<br>" + comentario;
					}
                    message.Subject = asunto;
                    contenido = contenido + "<br>" + "<br>" + "<br>" + "Saludos cordiales por parte de Oh my Dog!";
					var bodyBuilder = new BodyBuilder();
					bodyBuilder.HtmlBody = contenido;
					message.Body = bodyBuilder.ToMessageBody();

					using (var client = new MailKit.Net.Smtp.SmtpClient())
					{
						client.Connect("sandbox.smtp.mailtrap.io", 587, false);
						client.Authenticate("7472fca358e9d7", "4a53ab261e38ad");
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





















		private bool TurnosExists(int id)
        {
          return (_context.Turnos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
