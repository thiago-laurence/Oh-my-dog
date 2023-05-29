using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class UsuarioPerdidaPublicacion
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdPublicacion { get; set; }

    public string? Descripcion { get; set; }

    public DateTime Fecha { get; set; }

    public virtual Publicacion IdPublicacionNavigation { get; set; } = null!;
}
