using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class VacunaPerro
{
    public int Id { get; set; }

    public int IdVacuna { get; set; }

    public int IdPerro { get; set; }

    public string Dosis { get; set; } = null!;

    public string NroLote { get; set; } = null!;

    public DateTime FechaAplicacion { get; set; }

    public virtual Perros IdPerroNavigation { get; set; } = null!;

    public virtual Vacuna IdVacunaNavigation { get; set; } = null!;
}
