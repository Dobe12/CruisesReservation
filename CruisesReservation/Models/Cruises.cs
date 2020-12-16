using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CruisesReservation.Models
{
    public class Cruises
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public virtual ICollection<Ship> Ships { get; set; }

    }
}
