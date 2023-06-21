using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class HorarioTurno
{
    public int Id { get; set; }

    public string Turno { get; set; } = null!;

    public virtual ICollection<Turnos> Turnos { get; set; } = new List<Turnos>();
}
