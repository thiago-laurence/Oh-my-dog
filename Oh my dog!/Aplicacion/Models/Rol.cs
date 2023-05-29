using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Rol
{
    public int IdRol { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
