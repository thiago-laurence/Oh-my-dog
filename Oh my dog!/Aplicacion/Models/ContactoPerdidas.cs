namespace Aplicacion.Models
{
    public class ContactoPerdidas
    {
        public int Id { get; set; }
        public int IdPerdida { get; set; }
        public string EmailRemitente { get; set; } = null!;
    }
}
