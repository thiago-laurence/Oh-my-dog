using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Turnos
{
    public int Id { get; set; }

    public string? Motivo { get; set; }

    public int Estado { get; set; }

    public DateTime Fecha { get; set; }

    public int Dueno { get; set; }

    public int Horario { get; set; }

    public virtual EstadoTurno EstadoNavigation { get; set; } = null!;

    public virtual HorarioTurno HorarioNavigation { get; set; } = null!;

    public virtual ICollection<PerroTurnos> PerroTurnos { get; set; } = new List<PerroTurnos>();
}
