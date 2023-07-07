namespace Aplicacion.Models
{
    public partial class ContactoAdopciones
    {
        public int Id { get; set; }
        public int IdAdopcion { get; set; }
        public string EmailRemitente { get; set; } = null!;
    }
}
