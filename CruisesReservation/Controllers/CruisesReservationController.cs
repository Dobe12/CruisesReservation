using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CruisesReservation.Data;
using CruisesReservation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CruisesReservation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CruisesReservationController : ControllerBase
    {
        private readonly CruisesContext _context;

        public CruisesReservationController(CruisesContext context)
        {
            _context = context;
        }

        private IQueryable<Cruises> AllCinemas
        {
            get
            {
                return _context.Cinemas
                    .Include(c => c.Ships)
                    .ThenInclude(h => h.Seats);
            }
        }

        // GET api/[controller]/cruises
        [HttpGet("cruises")]
        [ProducesResponseType(typeof(IEnumerable<Cruises>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get()
        {
            var allCruises = await AllCinemas
                .ToListAsync();

            if (allCruises == null )
            {
                NotFound("Not found cruises");
            }

            return Ok(allCruises);
        }

        // GET api/[controller]/ships/{cruisesId}
        [HttpGet("ships/{cruisesId}")]
        [ProducesResponseType(typeof(IEnumerable<Ship>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetShips(int cruisesId)
        {
            var ships = await _context.Ships.Where(h => h.CruisesId == cruisesId).ToListAsync();

            if (ships == null)
            {
                NotFound("Ships not found");
            }

            return Ok(ships);
        }

        // GET api/[controller]/seats/{cruisesId}/{shipId}
        [HttpGet("seats/{cruisesId}/{shipId}")]
        [ProducesResponseType(typeof(IEnumerable<Seat>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetSeats(int cruisesId, int shipId)
        {
            var seats = await GetSeatsByCruisesIdAndShipId(cruisesId, shipId);

            return Ok(seats);
        }

        // POST api/[controller]/{cruisesId}/{seatRow}/{seatPlace}/{seatId}
        [HttpPost("{cruisesId}/{shipId}/{seatRow}/{seatPlace}/{phone}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Reservation(int cruisesId, int shipId, int seatRow, int seatPlace, double phone)
        {
            var seats = await GetSeatsByCruisesIdAndShipId(cruisesId, shipId);
            var seat = seats.FirstOrDefault(s => s.Row == seatRow && s.Place == seatPlace);

            if (seat == null)
            {
                NotFound("Seat not exist");
            }

            if (seat.IsReserved)
            {
                BadRequest("Seat already reserved");
            }

            seat.IsReserved = true;
            seat.PlaceHolderPhone = phone;

            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST api/[controller]/{cruisesId}/{shipId}/{seatId}
        [HttpGet("{cruisesId}/{shipId}/{seatId}/{phone}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Reservation(int cruisesId, int shipId, int seatId, double phone)
        {
            var seats = await GetSeatsByCruisesIdAndShipId(cruisesId, shipId);
            var seat = seats.FirstOrDefault(s => s.Id == seatId);

            if (seat == null)
            {
                NotFound("Seat not exist");
            }

            if (seat.IsReserved)
            {
                BadRequest("Seat already reserved");
            }

            seat.IsReserved = true;
            seat.PlaceHolderPhone = phone;

            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST api/[controller]/{cruisesId}/{seatId}/{phone}/remove
        [HttpPost("{cruisesId}/{shipId}/{seatId}/{phone}/remove")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RemoveReservation(int cruisesId, int shipId, int seatId, double phone)
        {
            var seats = await GetSeatsByCruisesIdAndShipId(cruisesId, shipId);
            var seat = seats.FirstOrDefault(s => s.Id == seatId);

            if (seat == null)
            {
                NotFound("Seat not exist");
            }

            if (!seat.IsReserved && seat.PlaceHolderPhone != phone)
            {
                BadRequest("Error. No rights");
            }

            seat.IsReserved = false;
            seat.PlaceHolderPhone = null;

            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST api/[controller]/{cruisesId}/{seatRow}/{seatPlace}/{seatId}/remove
        [HttpPost("{cruisesId}/{shipId}/{seatRow}/{seatPlace}/{phone}/remove")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RemoveReservation(int cruisesId, int shipId, int seatRow, int seatPlace, double phone)
        {
            var seats = await GetSeatsByCruisesIdAndShipId(cruisesId, shipId);
            var seat = seats.FirstOrDefault(s => s.Row == seatRow && s.Place == seatPlace);

            if (seat == null)
            {
                NotFound("Seat not exist");
            }

            if (!seat.IsReserved && seat.PlaceHolderPhone != phone)
            {
                BadRequest("Error. No rights");
            }

            seat.IsReserved = false;
            seat.PlaceHolderPhone = null;

            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET api/[controller]/reservations/{phone}
        [HttpGet("reservations/{phone}")]
        [ProducesResponseType(typeof(IEnumerable<Cruises>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetReservations(double phone)
        {
            var reservations = await _context.Cinemas
                .Include(c => c.Ships
                    .Where(h => h.Seats.Any(s => s.PlaceHolderPhone == phone)))
                .ThenInclude(h => h.Seats
                    .Where(s => s.PlaceHolderPhone == phone))
                .ToListAsync();

            if (reservations == null)
            {
                NotFound("Reservations not found");
            }

            return Ok(reservations);
        }
        private async Task<IEnumerable<Seat>> GetSeatsByCruisesIdAndShipId(int cruisesId, int shipId)
        {
            var cruises = await AllCinemas.FirstOrDefaultAsync(c => c.Id == cruisesId);

            if (cruises == null)
            {
                NotFound("Cinema not found");
            }

            var ship = cruises.Ships.FirstOrDefault(h => h.Id == shipId);

            if (ship == null)
            {
                NotFound("Cruises not found");
            }

            var seats = ship.Seats;

            if (seats == null)
            {
                NotFound("Seats not found");
            }

            return seats;
        }
    }
}