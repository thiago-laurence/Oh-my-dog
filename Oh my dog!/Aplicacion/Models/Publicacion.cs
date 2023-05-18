using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Publicacion
{
    public int Id { get; set; }

    public bool Estado { get; set; }

    public int Tipo { get; set; }

    public int IdPerro { get; set; }

    public virtual Perro IdPerroNavigation { get; set; } = null!;

    public virtual TipoPublicacion TipoNavigation { get; set; } = null!;

    public virtual ICollection<UsuarioAdopcionPublicacion> UsuarioAdopcionPublicacions { get; set; } = new List<UsuarioAdopcionPublicacion>();

    public virtual ICollection<UsuarioColectaPublicacion> UsuarioColectaPublicacions { get; set; } = new List<UsuarioColectaPublicacion>();

    public virtual ICollection<UsuarioPerdidaPublicacion> UsuarioPerdidaPublicacions { get; set; } = new List<UsuarioPerdidaPublicacion>();
}
