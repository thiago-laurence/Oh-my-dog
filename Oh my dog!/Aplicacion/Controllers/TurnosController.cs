using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Models;

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
