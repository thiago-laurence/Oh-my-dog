using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Models;
using NuGet.Versioning;

namespace Aplicacion.Controllers
{
    public class TinderdogController : Controller
    {
        private readonly OhmydogdbContext _context;

        public TinderdogController(OhmydogdbContext context)
        {
            _context = context;
        }

        // GET: Tinderdog
        public IActionResult Index()
        {
            ViewBag.ActiveView = "Tinderdog";
            ViewBag.SubView = "Sugerencias";
            string? email = User.FindFirst("Email")?.Value;
            return _context.Perros != null ?
                          View(_context.Usuarios.Where(u => u.Email == email).Include(u => u.GetPerros.Where(p => p.Estado == true)).First().GetPerros.OrderBy(p => p.Nombre).ToList()) :
                          Problem("Entity set 'OhmydogdbContext.Perros'  is null.");
        }

        public IActionResult MisCandidatos()
        {
            ViewBag.ActiveView = "Tinderdog";
            ViewBag.SubView = "MisCandidatos";
            string? email = User.FindFirst("Email")?.Value;
            return _context.Perros != null ?
                          View("MisCandidatos", _context.Usuarios.Where(u => u.Email == email).Include(u => u.GetPerros.Where(p => p.Estado == true)).First().GetPerros.OrderBy(p => p.Nombre).ToList()) :
                          Problem("Entity set 'OhmydogdbContext.Perros'  is null.");
        }


        [HttpGet]
        public IActionResult ListarSugerencias(int idPerro)
        {
            var perro = _context.Perros.Where(p => p.Id == idPerro).Include(p => p.MeGustaDados).Include(p => p.NoMeGustaDados).FirstOrDefault();
            var sugerencias = _context.Perros.Where(p => (p.IdDueno != perro!.IdDueno) && (p.Sexo != perro.Sexo) && !(perro.MeGustaDados.Select(m => m.IdPerroReceptor).Contains(p.Id)) && !(perro.NoMeGustaDados.Select(m => m.IdPerroReceptor).Contains(p.Id))); 
            var ordenamiento = sugerencias.OrderByDescending(p => p.Raza == perro!.Raza && p.Color == perro.Color)
                                          .ThenByDescending(p => p.Raza == perro!.Raza)
                                          .ThenByDescending(p => p.Color == perro!.Color);
            return (PartialView("_ListarSugerencias", ordenamiento.Take(2).ToList()));
        }

        [HttpPost]
        public JsonResult MeGusta(int idPerroEmisor, int idPerroReceptor)
        {
            _context.PerrosMeGusta.Add(new PerrosMeGusta { IdPerroEmisor = idPerroEmisor, IdPerroReceptor = idPerroReceptor });
            _context.SaveChanges();

            var match = _context.PerrosMeGusta.Where(p => p.IdPerroEmisor == idPerroReceptor).Any(m => m.IdPerroReceptor == idPerroEmisor);
            if (match)
            {
                return (Json(new { match = true }));
            }

            return (Json(new { match = false }));
        }

        [HttpPost]
        public JsonResult NoMeGusta(int idPerroEmisor, int idPerroReceptor)
        {
            _context.PerrosNoMeGusta.Add(new PerrosNoMeGusta { IdPerroEmisor = idPerroEmisor, IdPerroReceptor = idPerroReceptor });
            _context.SaveChanges();

            return (Json(new { success = true }));
        }
    }
}
