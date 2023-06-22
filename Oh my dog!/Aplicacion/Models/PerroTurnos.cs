using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class PerroTurnos
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int? IdPerro { get; set; }

    public int IdTurno { get; set; }

    public virtual Perro? IdPerroNavigation { get; set; }

    public virtual Turnos IdTurnoNavigation { get; set; } = null!;
}
