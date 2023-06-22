using Aplicacion.Data;
using System;
namespace Aplicacion.Models;
public class MeGustasRecibidosViewModel
{
    public Perro Perro { get; set; }
    public List<Perro> GetMeGustas { get; set; }

}