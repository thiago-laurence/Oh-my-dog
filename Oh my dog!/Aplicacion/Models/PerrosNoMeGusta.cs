using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class PerrosNoMeGusta
{
    public int Id { get; set; }

    public int IdPerroEmisor { get; set; }

    public int IdPerroReceptor { get; set; }

    public virtual Perros PerroEmisor { get; set; } = null!;

    public virtual Perros PerroReceptor { get; set; } = null!;

}