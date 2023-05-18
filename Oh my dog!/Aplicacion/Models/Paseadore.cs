using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Paseadore
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Foto { get; set; }

    public string Ubicacion { get; set; } = null!;

    public TimeSpan HorarioIn { get; set; }

    public TimeSpan HorarioOut { get; set; }
}
