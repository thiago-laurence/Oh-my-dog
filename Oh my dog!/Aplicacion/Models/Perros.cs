using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Perros
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Peso { get; set; }

    public DateTime? Celo { get; set; }

    public string Sexo { get; set; } = null!;

    public string? Foto { get; set; }

    public string Color { get; set; } = null!;

    public bool Estado { get; set; }

    public string Raza { get; set; } = null!;
    public string Observaciones { get; set; } = null!;

    public DateTime FechaDeNacimiento { get; set; }

    public int IdDueno { get; set; }
    public virtual Usuarios Dueno { get; set; } = null!;

    public virtual ICollection<Publicacion> Publicacions { get; set; } = new List<Publicacion>();

    public virtual ICollection<TratamientoPerro> TratamientoPerros { get; set; } = new List<TratamientoPerro>();

    public virtual ICollection<VacunaPerro> VacunaPerros { get; set; } = new List<VacunaPerro>();

    public virtual PublicacionTinderdog PublicacionTinderdog { get; set; } = null!;

    public virtual ICollection<PerroTurnos> PerroTurnos { get; set; } = new List<PerroTurnos>();
}
