namespace Aplicacion.Models;

public partial class Perdidas
{
    public int Id { get; set; }

    public string Email { get; set; }

    public string Nombre { get; set; }

    public string Peso { get; set; }

    public string Sexo { get; set; }

    public string Raza { get; set; }

    public string Color { get; set; }

    public string Descripcion { get; set; }
    public string? Foto { get; set; } = null;

    public DateTime FechaPerdida { get; set; }

    public int Estado { get; set; }

    public int Baja { get; set; }
}