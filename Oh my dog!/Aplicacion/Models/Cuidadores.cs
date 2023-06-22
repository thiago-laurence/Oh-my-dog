﻿using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Cuidadores
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public TimeSpan HorarioIn { get; set; }

    public TimeSpan HorarioOut { get; set; }

    public string? Foto { get; set; }

    public string Latitud { get; set; } = null!;

    public string Longitud { get; set; } = null!;

    public string Ubicacion { get; set; } = null!;

    public virtual ICollection<ModalidadCuidador> ModalidadCuidadors { get; set; } = new List<ModalidadCuidador>();
}
