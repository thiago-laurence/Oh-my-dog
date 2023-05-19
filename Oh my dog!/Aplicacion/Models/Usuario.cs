﻿using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public int Estado { get; set; }

    public int? IdRol { get; set; }

    public string Pass { get; set; } = null!;

    public virtual Rol? IdRolNavigation { get; set; }
}
