using Aplicacion.Data;

namespace Aplicacion.Models
{
    public class PerdidasViewModel
    {
        public IEnumerable<Perdidas> Perdidas { get; set; }
        public Perdidas perdida { get; set; }
        public Paginacion<Perdidas> Paginacion { get; set; }
        public bool Origen { get; set; }
    }
}