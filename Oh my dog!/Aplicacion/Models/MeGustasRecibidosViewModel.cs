using Aplicacion.Data;
using System;
namespace Aplicacion.Models;
public class MeGustasRecibidosViewModel
{
    public Perros Perro { get; set; }
    public List<Perros> GetMeGustas { get; set; }

}