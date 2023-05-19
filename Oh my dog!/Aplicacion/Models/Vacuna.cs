using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Vacuna
{
    public int Id { get; set; }

    public string? Vacuna1 { get; set; }

    public virtual ICollection<VacunaPerro> VacunaPerros { get; set; } = new List<VacunaPerro>();
}
