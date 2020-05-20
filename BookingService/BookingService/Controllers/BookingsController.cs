using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BookingService;

namespace BookingService.Controllers
{
    [RoutePrefix("api/Bookings")]//Grund urlen
    public class BookingsController : ApiController
    {
        private BookingModel db = new BookingModel();

        // GET: api/Bookings
        [Route("")]
        public IQueryable<Bookings> GetBookings()
        {
            return db.Bookings;
        }
        //H�mtar alla bes�kare p� ett specifikt event
        [Route("Event/{eId:int}/Visitor")]//Hur urlen skall se ut
     
        public IQueryable<Bookings> GetVisitorOnEvent(int eId)
        {
            return db.Bookings.Where(s => s.Event_Id == eId).Where(e => e.User_Type == "Bes�kare");
        }
        //H�mtar alla volont�rer p� ett specifikt event
        [Route("Event/{eId:int}/Volounteer")]

        public IQueryable<Bookings> GetVolounteerOnEvent(int eId)
        {
            return db.Bookings.Where(s => s.Event_Id == eId).Where(e => e.User_Type == "Volont�r");
        }

        [Route("User/{uId:int}")]

        public IQueryable<Bookings> GetBookingFromUser(int uId)
        {
            return db.Bookings.Where(s => s.User_Id == uId);
        }

        // GET: api/Bookings/5
        [Route("{id:int}")]
        [ResponseType(typeof(Bookings))]
        public IHttpActionResult GetBookings(int id)
        {
            Bookings bookings = db.Bookings.Find(id);
            
            if (bookings == null)
            {
                return NotFound();
            }
            return Ok(bookings);
        }

        //should only be for admins?
        // PUT: api/Bookings/5
        [Route("{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBookings(int id, Bookings bookings)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bookings.Booking_Id)
            {
                return BadRequest();
            }

            db.Entry(bookings).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        //'subscribe' to new event as either attende or 
        // POST: api/Bookings
        [Route("")]
        [ResponseType(typeof(Bookings))]
        public IHttpActionResult PostBookings(Bookings bookings)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                db.Bookings.Add(bookings);
                db.SaveChanges();

                return CreatedAtRoute("DefaultApi", new { id = bookings.Booking_Id }, bookings);
            }
            catch(Exception e)
            {
                Console.Write(e);
                throw;
            }
           
        }

        // DELETE: api/Bookings/5
        [Route("{id:int}")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult DeleteBookings(int id)
        {
            Bookings bookings = db.Bookings.Find(id);
            if (bookings == null)
            {
                return NotFound();
            }

            db.Bookings.Remove(bookings);
            db.SaveChanges();

            return Ok(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookingsExists(int id)
        {
            return db.Bookings.Count(e => e.Booking_Id == id) > 0;
        }
    }
}