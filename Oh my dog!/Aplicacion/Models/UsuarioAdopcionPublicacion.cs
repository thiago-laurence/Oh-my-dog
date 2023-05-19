using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class UsuarioAdopcionPublicacion
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdPublicacion { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual Publicacion IdPublicacionNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
