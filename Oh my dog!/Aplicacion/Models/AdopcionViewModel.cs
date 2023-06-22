using Aplicacion.Data;

namespace Aplicacion.Models
{
    public class AdopcionViewModel
    {
        public IEnumerable<Adopciones> Adopciones { get; set; }
        public Adopciones adopcion { get; set; }
        public Paginacion<Adopciones> Paginacion { get; set; }
        public bool Origen { get; set; }


    }
}
