using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Tratamiento
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<TratamientoPerro> TratamientoPerros { get; set; } = new List<TratamientoPerro>();
}
