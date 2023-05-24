using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aplicacion.Models;
using System.Text.RegularExpressions;
using Aplicacion.Data;

namespace Aplicacion.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly OhmydogdbContext _context;

        public UsuariosController(OhmydogdbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index(string buscar, int? numeroPagina, string filtroActual)
        {
            if (_context.Usuarios != null)
            {
                var usuarios = from usuario in _context.Usuarios select usuario;
                if (buscar != null)
                {
                    numeroPagina = 1;
                }
                else
                {
                    buscar = filtroActual;
                }
                if (!String.IsNullOrEmpty(buscar))
                {
                    usuarios = usuarios.Where(u => u.Apellido.Contains(buscar));
                }
                ViewData["FiltroActual"] = filtroActual;
                int cantidadRegistros = 5;
                UsuarioViewModel modelo = new UsuarioViewModel
                {
                    Usuario = new Usuario(),
                    ListUsuarios = await usuarios.ToListAsync(),
                    Paginacion = await Paginacion<Usuario>.CrearPaginacion(usuarios.AsNoTracking(), numeroPagina ?? 1, cantidadRegistros)
                };

                return View(modelo);
            }
            return (Problem("Entity set 'OhmydogdbContext.Usuarios'  is null."));

            //return ( (_context.Usuarios != null) ? View(await _context.Usuarios.ToListAsync()) : Problem("Entity set 'OhmydogdbContext.Usuarios'  is null."));
        }

        /*public async Task<IActionResult> Index(string buscar)
        {
            var usuarios = from usuario in _context.Usuarios select usuario;

            if (!String.IsNullOrEmpty(buscar))
            {
                usuarios = usuarios.Where(u => u.Apellido.Contains(buscar));
            }

            return (View(await usuarios.ToListAsync()));
        }*/

        public IActionResult CambiarContrasena()
        {
            return (View());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            //return Json(usuario);
            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Nombre,Apellido,Direccion,Telefono,Estado,Rol,Contrasena")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (validarEmail(usuario.Email))
                {
                    ModelState.AddModelError("Email", "El email ingresado ya está registrado");
                    //return View(usuario);
                    return RedirectToAction(nameof(Index));
                }
                TempData["RegistroExitoso"] = true;
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        /*public async Task<ActionResult<Usuario>> getUsuario(int id)
        {
            return (await )
        }*/
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,Nombre,Apellido,Direccion,Telefono,Estado,Rol,Contrasena")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (validarEmail(usuario.Email))
                {
                    ModelState.AddModelError("Email", "El email ingresado ya está registrado");
                    return View(usuario);
                }

                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'OhmydogdbContext.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool validarEmail(String email)
        {
            return (_context.Usuarios?.Any(u => u.Email == email)).GetValueOrDefault();
        }

        private bool validarContrasena(String pass)
        {
            return (
                    (pass.Length >= 8) && (Regex.IsMatch(pass, @"\d")) && (Regex.IsMatch(pass, @"[a-zA-Z]")) && (Regex.IsMatch(pass, @"\W"))
                );
        }
    }
}