using System;
namespace Aplicacion.Models
{
    public class PaseadorViewModel
    {
        public IEnumerable<Paseadores> ListPaseadores { get; set; }

        public Paseadores Paseador { get; set; }
    }
}
