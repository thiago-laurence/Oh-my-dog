using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class PerroTurno
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int? IdPerro { get; set; }

    public virtual Perros? IdPerroNavigation { get; set; }
}
