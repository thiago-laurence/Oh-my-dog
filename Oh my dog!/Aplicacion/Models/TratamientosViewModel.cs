using Aplicacion.Data;
using System;
namespace Aplicacion.Models;
public class TratamientosViewModel
{
    public Perro Perro { get; set; }
    public List<Tratamiento> Tratamientos{ get; set; }

}
