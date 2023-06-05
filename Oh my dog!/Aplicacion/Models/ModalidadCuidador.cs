using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class ModalidadCuidador
{
    public int Id { get; set; }

    public int IdCuidador { get; set; }

    public int IdModalidad { get; set; }

    public virtual Cuidadores IdCuidadorNavigation { get; set; } = null!;

    public virtual TipoModalidad IdModalidadNavigation { get; set; } = null!;
}
