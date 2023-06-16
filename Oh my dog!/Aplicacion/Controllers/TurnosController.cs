using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Models;
using System.Text.Json;

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
			 int idDueño = _context.Usuarios.Where(m => m.Email == mail).Select(i => i.Id).First();
            var perrosUsuario = _context.Perros.Where(c => c.IdDueño == idDueño && c.Estado==true).ToList();
            Console.Write(perrosUsuario.Count());
			return JsonSerializer.Serialize(perrosUsuario);
		}

        public IActionResult obtenerEventos(String mail)
		{
			int idDueño = _context.Usuarios.Where(m => m.Email == mail).Select(i => i.Id).First();
			int? rol = _context.Usuarios.Where(m => m.Email == mail).Select(i => i.IdRol).First();
			
			if (rol == 1) {
				List<Turnos> turnosAux = _context.Turnos.ToList();

				
				return Json(new { admin = true, turnos= JsonSerializer.Serialize(turnosAux) });
			}
            else
            {
				List<Turnos> turnoAux = _context.Turnos.Where(t =>  idDueño == t.Dueno).ToList();
                
			
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
			_context.Turnos.Add(turno);
			_context.SaveChanges();
            
			int id_Turno = _context.Turnos.Max(i=>i.Id); // Retrieve the generated ID from the newly inserted Turnos record

			perros.ForEach(perro => {
				PerroTurnos turnoPerro = new PerroTurnos();
				turnoPerro.Nombre = perro;
				turnoPerro.IdTurno = id_Turno;
                int? idPerro = _context.Perros
                    .Where(m => m.IdDueño == turno.Dueno && m.Nombre == perro)?
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
            Turnos turnoAux= new Turnos();
            turnoAux.Estado=turno.Estado;
            turnoAux.Motivo=turno.Motivo;
            turnoAux.Dueno=turno.Dueno;
			turnoAux.Fecha =turno.Fecha ;
            turnoAux.Horario=turno.Horario;
            turnoAux.Id = turno.Id;
			return Json(new{asignado= JsonSerializer.Serialize(turnoAux)});
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
        public IActionResult rechazarTurno(int idTurno)
        {

			Turnos turno = _context.Turnos.Where(m => m.Id == idTurno).First();
			turno.Estado = 2;
			Turnos borrado = _context.Turnos.Where(m => m.Id == idTurno).First();
			List<PerroTurnos> turnosPerrosBorrar = _context.PerroTurnos.Where(m => m.IdPerro == idTurno).ToList();
			turnosPerrosBorrar.ForEach(turno =>
			{
				_context.PerroTurnos.Remove(turno);
			});
			_context.SaveChanges();
			_context.Turnos.Remove(borrado);
			_context.SaveChanges();
			turno.Id = 0;
			
			_context.Turnos.Add(turno);
			_context.SaveChanges();

			return Json(true);
        }

		[HttpPost]
		public IActionResult aceptarTurno(int idTurno)
		{
            Turnos turno = _context.Turnos.Where(m => m.Id == idTurno).First();
            turno.Estado = 1;
            Turnos borrado= _context.Turnos.Where(m => m.Id == idTurno).First();
			List<PerroTurnos> turnosPerrosBorrar = _context.PerroTurnos.Where(m => m.IdPerro == idTurno).ToList();
			turnosPerrosBorrar.ForEach(turno =>
			{
				_context.PerroTurnos.Remove(turno);
			});
			_context.SaveChanges();
			_context.Turnos.Remove(borrado);
			_context.SaveChanges();
			turno.Id = 0;
			_context.Turnos.Add(turno);

            _context.SaveChanges();
			return Json(true);
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

        private bool TurnosExists(int id)
        {
          return (_context.Turnos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
