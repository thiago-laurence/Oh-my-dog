using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Turno
{
    public int Id { get; set; }

    public string? Motivo { get; set; }

    public int Estado { get; set; }

    public DateTime Fecha { get; set; }

    public int Dueno { get; set; }

    public int Perro { get; set; }

    public virtual Usuario DuenoNavigation { get; set; } = null!;

    public virtual EstadoTurno EstadoNavigation { get; set; } = null!;

    public virtual Perro PerroNavigation { get; set; } = null!;
}
