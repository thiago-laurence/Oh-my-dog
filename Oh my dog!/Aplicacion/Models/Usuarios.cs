using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class Usuarios
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public bool Estado { get; set; }

    public bool IdRol { get; set; }

    public string Pass { get; set; } = null!;

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();

    public virtual ICollection<UsuarioAdopcionPublicacion> UsuarioAdopcionPublicacions { get; set; } = new List<UsuarioAdopcionPublicacion>();

    public virtual ICollection<UsuarioColectaPublicacion> UsuarioColectaPublicacions { get; set; } = new List<UsuarioColectaPublicacion>();

    public virtual ICollection<UsuarioPerdidaPublicacion> UsuarioPerdidaPublicacions { get; set; } = new List<UsuarioPerdidaPublicacion>();
}
