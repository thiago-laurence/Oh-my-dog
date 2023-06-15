using Aplicacion.Data;
using Aplicacion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion.Controllers
{
    public class AdopcionesController : Controller
    {
        private readonly OhmydogdbContext _context;

        public AdopcionesController(OhmydogdbContext context)
        {
            _context = context;
        }
        private int cantidadDeRegistros = 3;
        public IActionResult PublicarAdopcionesIndex()
        {
            ViewBag.ActiveView = "PublicarAdopcion";
            return View();
        }

        [HttpPost]  
        public async Task<IActionResult> PublicarAdopcionesIndex([Bind("Id,Email,Nombre,Peso,Sexo,Foto,Raza,Color,Descripcion,Estado")] Adopciones adopcion) {
            
            adopcion.Foto = "Hardcode";
            if (hayAdopcionConEmailYNombre(adopcion.Email, adopcion.Nombre)) 
            {
                ViewBag.Message = "Adopcion fallida. Ya existe una adopcion para este usuario con ese nombre de perro, por favor elija otro nombre";
                return View();
            }
            _context.Add(adopcion);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> IndexAdopciones(string query, int? numeroPagina)
        {
            var adopciones = from adopcion in _context.Adopciones select adopcion;

            if (query != null && numeroPagina == null)
            {
                numeroPagina = 1;
            }
            if (!String.IsNullOrEmpty(query))
            {
                adopciones = adopciones.Where(a => a.Nombre.Contains(query));
            }

            adopciones = adopciones.OrderBy(a => a.Nombre);

            AdopcionViewModel modelo = new AdopcionViewModel
            {
                Origen = (query != null) ? true : false,
                Paginacion = await Paginacion<Adopciones>.CrearPaginacion(adopciones.AsNoTracking(), numeroPagina ?? 1, cantidadDeRegistros)
            };
            return (PartialView("_ListarAdopciones", modelo));
        }

        public bool hayAdopcionConEmailYNombre(string email, string nombre) {
            Adopciones adopcion = _context.Adopciones.Where(a => a.Email == email && a.Nombre == nombre).FirstOrDefault();
            if (adopcion != null) 
            { 
                return true;
            }
            return false;
        }
    }
}
