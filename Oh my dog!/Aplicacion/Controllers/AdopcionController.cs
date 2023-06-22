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
    public class AdopcionController : Controller
    {
        private readonly OhmydogdbContext _context;

        public AdopcionController(OhmydogdbContext context)
        {
            _context = context;
        }

        // GET: Adopcion
        public async Task<IActionResult> Index()
        {
            var ohmydogdbContext = _context.UsuarioAdopcionPublicacions.Include(u => u.IdPublicacionNavigation);
            return View(await ohmydogdbContext.ToListAsync());
        }

        // GET: Adopcion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UsuarioAdopcionPublicacions == null)
            {
                return NotFound();
            }

            var usuarioAdopcionPublicacion = await _context.UsuarioAdopcionPublicacions
                .Include(u => u.IdPublicacionNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarioAdopcionPublicacion == null)
            {
                return NotFound();
            }

            return View(usuarioAdopcionPublicacion);
        }

        // GET: Adopcion/Create
        public IActionResult Create()
        {
            ViewData["IdPublicacion"] = new SelectList(_context.Publicacions, "Id", "Id");
            return View();
        }

        // POST: Adopcion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdUsuario,IdPublicacion,Descripcion")] UsuarioAdopcionPublicacion usuarioAdopcionPublicacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuarioAdopcionPublicacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPublicacion"] = new SelectList(_context.Publicacions, "Id", "Id", usuarioAdopcionPublicacion.IdPublicacion);
            return View(usuarioAdopcionPublicacion);
        }

        // GET: Adopcion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UsuarioAdopcionPublicacions == null)
            {
                return NotFound();
            }

            var usuarioAdopcionPublicacion = await _context.UsuarioAdopcionPublicacions.FindAsync(id);
            if (usuarioAdopcionPublicacion == null)
            {
                return NotFound();
            }
            ViewData["IdPublicacion"] = new SelectList(_context.Publicacions, "Id", "Id", usuarioAdopcionPublicacion.IdPublicacion);
            return View(usuarioAdopcionPublicacion);
        }

        // POST: Adopcion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdUsuario,IdPublicacion,Descripcion")] UsuarioAdopcionPublicacion usuarioAdopcionPublicacion)
        {
            if (id != usuarioAdopcionPublicacion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarioAdopcionPublicacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioAdopcionPublicacionExists(usuarioAdopcionPublicacion.Id))
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
            ViewData["IdPublicacion"] = new SelectList(_context.Publicacions, "Id", "Id", usuarioAdopcionPublicacion.IdPublicacion);
            return View(usuarioAdopcionPublicacion);
        }

        // GET: Adopcion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UsuarioAdopcionPublicacions == null)
            {
                return NotFound();
            }

            var usuarioAdopcionPublicacion = await _context.UsuarioAdopcionPublicacions
                .Include(u => u.IdPublicacionNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarioAdopcionPublicacion == null)
            {
                return NotFound();
            }

            return View(usuarioAdopcionPublicacion);
        }

        // POST: Adopcion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UsuarioAdopcionPublicacions == null)
            {
                return Problem("Entity set 'OhmydogdbContext.UsuarioAdopcionPublicacions'  is null.");
            }
            var usuarioAdopcionPublicacion = await _context.UsuarioAdopcionPublicacions.FindAsync(id);
            if (usuarioAdopcionPublicacion != null)
            {
                _context.UsuarioAdopcionPublicacions.Remove(usuarioAdopcionPublicacion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioAdopcionPublicacionExists(int id)
        {
          return (_context.UsuarioAdopcionPublicacions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
