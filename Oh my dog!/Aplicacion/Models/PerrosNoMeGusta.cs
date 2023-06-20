using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class PerrosNoMeGusta
{
    public int Id { get; set; }

    public int IdPerroEmisor { get; set; }

    public int IdPerroReceptor { get; set; }

    public virtual Perro PerroEmisor { get; set; } = null!;

    public virtual Perro PerroReceptor { get; set; } = null!;

}