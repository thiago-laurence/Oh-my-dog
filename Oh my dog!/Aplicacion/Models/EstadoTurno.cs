using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class EstadoTurno
{
    public int Id { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
