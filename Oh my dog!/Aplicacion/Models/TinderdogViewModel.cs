using Aplicacion.Data;
using System;
namespace Aplicacion.Models;
public class TinderdogViewModel
{
    public Perros Perro { get; set; } = null!;
    public List<PublicacionTinderdog> Publicaciones { get; set; } = null!;

}
