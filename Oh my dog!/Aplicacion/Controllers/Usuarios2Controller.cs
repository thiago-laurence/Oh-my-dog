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
    public class Usuarios2Controller : Controller
    {
        private readonly OhmydogContext _context;

        public Usuarios2Controller(OhmydogContext context)
        {
            _context = context;
        }

        // GET: Usuarios2
        public async Task<IActionResult> Index()
        {
              return _context.Usuarios2s != null ? 
                          View(await _context.Usuarios2s.ToListAsync()) :
                          Problem("Entity set 'OhmydogContext.Usuarios2s'  is null.");
        }

        // GET: Usuarios2/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios2s == null)
            {
                return NotFound();
            }

            var usuarios2 = await _context.Usuarios2s
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarios2 == null)
            {
                return NotFound();
            }

            return View(usuarios2);
        }

        // GET: Usuarios2/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios2/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] Usuarios2 usuarios2)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuarios2);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuarios2);
        }

        // GET: Usuarios2/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios2s == null)
            {
                return NotFound();
            }

            var usuarios2 = await _context.Usuarios2s.FindAsync(id);
            if (usuarios2 == null)
            {
                return NotFound();
            }
            return View(usuarios2);
        }

        // POST: Usuarios2/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Usuarios2 usuarios2)
        {
            if (id != usuarios2.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarios2);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Usuarios2Exists(usuarios2.Id))
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
            return View(usuarios2);
        }

        // GET: Usuarios2/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios2s == null)
            {
                return NotFound();
            }

            var usuarios2 = await _context.Usuarios2s
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarios2 == null)
            {
                return NotFound();
            }

            return View(usuarios2);
        }

        // POST: Usuarios2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios2s == null)
            {
                return Problem("Entity set 'OhmydogContext.Usuarios2s'  is null.");
            }
            var usuarios2 = await _context.Usuarios2s.FindAsync(id);
            if (usuarios2 != null)
            {
                _context.Usuarios2s.Remove(usuarios2);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Usuarios2Exists(int id)
        {
          return (_context.Usuarios2s?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
