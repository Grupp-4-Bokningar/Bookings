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
        //Hämtar alla besökare på ett specifikt event
        [Route("Event/{eId:int}/Visitor")]//Hur urlen skall se ut
     
        public IQueryable<Bookings> GetVisitorOnEvent(int eId)
        {
            return db.Bookings.Where(s => s.Event_Id == eId).Where(e => e.User_Type == "Besökare");
        }
        //Hämtar alla volontärer på ett specifikt event
        [Route("Event/{eId:int}/Volounteer")]

        public IQueryable<Bookings> GetVolounteerOnEvent(int eId)
        {
            return db.Bookings.Where(s => s.Event_Id == eId).Where(e => e.User_Type == "Volontär");
        }

        //Ska hämta all info om en volontär :)
        [Route("User/{uId:int}")]

        public IQueryable<Bookings> GetBookingFromUser(int uId)
        {

                return db.Bookings.Where(s => s.User_Id == uId);

        }


        [Route("Visitor/{uId:int}")]
        public IQueryable<Bookings> GetAllFromUser(int uId)
        {
            try
            {
                return db.Bookings.Where(s => s.User_Id == uId);
            }
            catch (Exception e)
            {
                Console.Write(e);
                throw;          // senare problem
            }

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

                var temp = db.Bookings.Where(e => e.Event_Id == bookings.Event_Id).Where(u => u.User_Id == bookings.User_Id); //Kollar så det inte redan finns i databasen

                if(temp == null)
                {
                    db.Bookings.Add(bookings);
                    db.SaveChanges();

                    return CreatedAtRoute("DefaultApi", new { id = bookings.Booking_Id }, bookings);
                }
                else //Om användaren redan finns på eventet så skickas ett error meddellande tillbaks.
                {
                    return BadRequest("Du är redan anmäld på detta event.");
                }

                
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
        [Route("User/{uId:int}/Event/{eId:int}")]
        [ResponseType(typeof(Bookings))]
        public IHttpActionResult DeleteBookingWithUserIDAndEventID(int uId, int eId)
        {
            Bookings bookings = new Bookings();
            
            int bookingId = db.Bookings.Where(u => u.User_Id == uId).Where(e => e.Event_Id == eId).Select(i => i.Booking_Id).FirstOrDefault();
            bookings = db.Bookings.Find(bookingId);
           

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