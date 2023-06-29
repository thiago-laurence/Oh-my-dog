using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class PublicacionTinderdog
{
    public int Id { get; set; }

    public int IdPerro { get; set; }

    public virtual Perros Perro { get; set; } = null!;

}