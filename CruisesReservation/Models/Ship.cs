using System.Collections.Generic;

namespace CruisesReservation.Models
{
    public class Ship
    {
        public int Id { get; set; }
        public int Number { get; set; }

        public int? CruisesId { get; set; }

        public virtual ICollection<Seat> Seats { get; set; }
    }
}