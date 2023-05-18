using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public bool Estado { get; set; }

    public bool Rol { get; set; }

    public string Contrasena { get; set; } = null!;
}
