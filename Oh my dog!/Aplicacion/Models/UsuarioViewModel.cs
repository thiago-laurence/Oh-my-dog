using Aplicacion.Data;
using System;
namespace Aplicacion.Models;
public class UsuarioViewModel
{
    public IEnumerable<Usuario> ListUsuarios { get; set; }

    public Usuario Usuario { get; set; }
    public Paginacion<Usuario> Paginacion { get; set; } 

}