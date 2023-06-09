﻿using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class TratamientoPerro
{
    public int Id { get; set; }

    public int IdTratamiento { get; set; }

    public int IdPerro { get; set; }

    public string? Observaciones { get; set; }

    public DateTime Fecha { get; set; }

    public virtual Perros IdPerroNavigation { get; set; } = null!;

    public virtual Tratamientos IdTratamientoNavigation { get; set; } = null!;
}
