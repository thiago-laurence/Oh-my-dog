using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class TipoModalidad
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<ModalidadCuidador> ModalidadCuidadors { get; set; } = new List<ModalidadCuidador>();
}
