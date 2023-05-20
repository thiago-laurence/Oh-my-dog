using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class UsuarioColectaPublicacion
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdPublicacion { get; set; }

    public string Titulo { get; set; } = null!;

    public string Motivo { get; set; } = null!;

    public virtual Publicacion IdPublicacionNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
