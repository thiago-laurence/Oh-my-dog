using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class PerrosMeGusta
{
    public int Id { get; set; }

    public int IdPerroEmisor { get; set; }

    public int IdPerroReceptor { get; set; }

    public virtual PublicacionTinderdog PerroEmisor { get; set; } = null!;

    public virtual PublicacionTinderdog PerroReceptor { get; set; } = null!;

}