using Aplicacion.Data;
using System;
namespace Aplicacion.Models;
public class UsuarioViewModel
{
    public IEnumerable<Usuarios> ListUsuarios { get; set; }

    public Usuarios Usuario { get; set; }
    public Paginacion<Usuarios> Paginacion { get; set; } 

}