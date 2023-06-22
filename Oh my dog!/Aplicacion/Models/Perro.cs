using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Perro
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public float Peso { get; set; }

    public bool? Celo { get; set; }

    public string Sexo { get; set; } = null!;

    public string? Foto { get; set; }

    public string Color { get; set; } = null!;

    public bool Estado { get; set; }

    public string Raza { get; set; } = null!;

    public DateTime FechaDeNacimiento { get; set; }

    public int? IdDueno { get; set; }

    public virtual Usuarios? IdDuenoNavigation { get; set; }

    public virtual ICollection<PerroTurnos> PerroTurnos { get; set; } = new List<PerroTurnos>();

    public virtual ICollection<Publicacion> Publicacions { get; set; } = new List<Publicacion>();

    public virtual ICollection<TratamientoPerro> TratamientoPerros { get; set; } = new List<TratamientoPerro>();

    public virtual ICollection<VacunaPerro> VacunaPerros { get; set; } = new List<VacunaPerro>();
}
