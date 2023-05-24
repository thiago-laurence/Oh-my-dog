using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Rol
{
    public bool IdRol { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
