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
    public class PublicacionsController : Controller
    {
        private readonly OhmydogdbContext _context;

        public PublicacionsController(OhmydogdbContext context)
        {
            _context = context;
        }

        // GET: Publicacions
        public async Task<IActionResult> Index()
        {
            var ohmydogdbContext = _context.Publicacions.Include(p => p.IdPerroNavigation).Include(p => p.TipoNavigation);
            return View(await ohmydogdbContext.ToListAsync());
        }

        // GET: Publicacions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Publicacions == null)
            {
                return NotFound();
            }

            var publicacion = await _context.Publicacions
                .Include(p => p.IdPerroNavigation)
                .Include(p => p.TipoNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publicacion == null)
            {
                return NotFound();
            }

            return View(publicacion);
        }

        // GET: Publicacions/Create
        public IActionResult Create()
        {
            ViewData["IdPerro"] = new SelectList(_context.Perros, "Id", "Id");
            ViewData["Tipo"] = new SelectList(_context.TipoPublicacions, "Id", "Id");
            return View();
        }

        // POST: Publicacions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Estado,Tipo,IdPerro")] Publicacion publicacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(publicacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdPerro"] = new SelectList(_context.Perros, "Id", "Id", publicacion.IdPerro);
            ViewData["Tipo"] = new SelectList(_context.TipoPublicacions, "Id", "Id", publicacion.Tipo);
            return View(publicacion);
        }

        // GET: Publicacions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Publicacions == null)
            {
                return NotFound();
            }

            var publicacion = await _context.Publicacions.FindAsync(id);
            if (publicacion == null)
            {
                return NotFound();
            }
            ViewData["IdPerro"] = new SelectList(_context.Perros, "Id", "Id", publicacion.IdPerro);
            ViewData["Tipo"] = new SelectList(_context.TipoPublicacions, "Id", "Id", publicacion.Tipo);
            return View(publicacion);
        }

        // POST: Publicacions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Estado,Tipo,IdPerro")] Publicacion publicacion)
        {
            if (id != publicacion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(publicacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PublicacionExists(publicacion.Id))
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
            ViewData["IdPerro"] = new SelectList(_context.Perros, "Id", "Id", publicacion.IdPerro);
            ViewData["Tipo"] = new SelectList(_context.TipoPublicacions, "Id", "Id", publicacion.Tipo);
            return View(publicacion);
        }

        // GET: Publicacions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Publicacions == null)
            {
                return NotFound();
            }

            var publicacion = await _context.Publicacions
                .Include(p => p.IdPerroNavigation)
                .Include(p => p.TipoNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (publicacion == null)
            {
                return NotFound();
            }

            return View(publicacion);
        }

        // POST: Publicacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Publicacions == null)
            {
                return Problem("Entity set 'OhmydogdbContext.Publicacions'  is null.");
            }
            var publicacion = await _context.Publicacions.FindAsync(id);
            if (publicacion != null)
            {
                _context.Publicacions.Remove(publicacion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PublicacionExists(int id)
        {
          return (_context.Publicacions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
