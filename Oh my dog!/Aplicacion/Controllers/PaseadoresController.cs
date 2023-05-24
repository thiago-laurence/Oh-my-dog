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
    public class PaseadoresController : Controller
    {
        private readonly OhmydogdbContext _context;

        public PaseadoresController(OhmydogdbContext context)
        {
            _context = context;
        }

        // GET: Paseadores
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

        // POST: Paseadores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Email,Foto,Ubicacion,HorarioIn,HorarioOut,Latitud,Longitud")] Paseadore paseadore)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Email,Foto,Ubicacion,HorarioIn,HorarioOut,Latitud,Longitud")] Paseadore paseadore)
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

        // POST: Paseadores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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

        private bool PaseadoreExists(int id)
        {
          return (_context.Paseadores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
