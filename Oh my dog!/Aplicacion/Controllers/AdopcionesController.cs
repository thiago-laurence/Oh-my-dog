using Aplicacion.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aplicacion.Controllers
{
    public class AdopcionesController : Controller
    {
        private readonly OhmydogdbContext _context;

        public AdopcionesController(OhmydogdbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> PublicarAdopcionesIndex()
        {
            ViewBag.ActiveView = "PublicarAdopcion";
            return View();
        }

        public async Task<IActionResult> Create(Adopciones adopcion) {
            if (ModelState.IsValid)
            {
               _context.Add(adopcion);
               await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
