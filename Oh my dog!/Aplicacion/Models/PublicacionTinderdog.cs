using System;
using System.Collections.Generic;

namespace Aplicacion.Models;

public partial class PublicacionTinderdog
{
    public int Id { get; set; }

    public int IdPerro { get; set; }

    public string Descripcion { get; set; } = "";

    public virtual Perros Perro { get; set; } = null!;

    public virtual ICollection<FotosPublicacionTinderdog> Fotos { get; set; } = null!;

    public virtual ICollection<PerrosMeGusta> MeGustaDados { get; set; } = new List<PerrosMeGusta>();

    public virtual ICollection<PerrosMeGusta> MeGustaRecibidos { get; set; } = new List<PerrosMeGusta>();

    public virtual ICollection<PerrosNoMeGusta> NoMeGustaDados { get; set; } = new List<PerrosNoMeGusta>();

    public virtual ICollection<PerrosNoMeGusta> NoMeGustaRecibidos { get; set; } = new List<PerrosNoMeGusta>();
}