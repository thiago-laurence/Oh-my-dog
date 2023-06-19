using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Adopciones
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Tamano { get; set; } = null!;

    public string Sexo { get; set; } = null!;

    public string Raza { get; set; } = null!;

    public string Color { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int Estado { get; set; }

    public int Baja { get; set; }
}
