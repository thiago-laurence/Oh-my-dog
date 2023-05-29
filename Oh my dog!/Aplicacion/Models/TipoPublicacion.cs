using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class TipoPublicacion
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Publicacion> Publicacions { get; set; } = new List<Publicacion>();
}
