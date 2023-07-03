using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class FotosPublicacionTinderdog
{
    public int Id { get; set; }

    public int IdPublicacion { get; set; }

    public string Foto { get; set; } = "";

    public virtual PublicacionTinderdog Publicacion { get; set; } = null!;
}
