using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using BookingService;
using BookingService.Models;
using Newtonsoft.Json;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace BookingService.Controllers
{
    [RoutePrefix("api/Bookings")]//Grund urlen
    public class BookingsController : ApiController
    {
        private BookingModel db = new BookingModel();
        string baseURLEvent = "http://193.10.202.77/EventService/api/";

        // GET: api/Bookings
        [Route("")]
        public List<BookingModell> GetBookings()
        {
            List<BookingModell> booksList = new List<BookingModell>();
            List<EventModell> eventList = new List<EventModell>();
            EventModell tempEvent = new EventModell();

            var sql = db.Bookings.Select(s => s.Booking_Id).ToArray();

            for (var i = 0; i < sql.Count(); i++)
            {
                var temp = sql[i];
                tempEvent = GetEvent(db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.Event_Id).FirstOrDefault()).Result;
                int user = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Id).FirstOrDefault();
                string userType = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Type).FirstOrDefault();

                booksList.Add(new BookingModell
                {
                     Booking_Id = temp,
                     EventNamn = tempEvent.Event_Name,
                     Startdate = tempEvent.Event_Start_Datetime,
                     Enddate  = tempEvent.Event_End_Datetime,
                     User_Name = user.ToString(),
                     User_Type = userType
                });

            }
            return booksList;
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

        //Ska hämta all info om en besökare/volontär :)
        [Route("User/{uId:int}")]

        public IQueryable<Bookings> GetBookingFromUser(int uId)
        {

                return db.Bookings.Where(s => s.User_Id == uId);

        }


        // GET: api/Bookings/5
        [Route("{id:int}")]
        [ResponseType(typeof(BookingModell))]
        public IHttpActionResult GetBookings(int id)
        {
            Bookings bookings = db.Bookings.Find(id);
            BookingModell resBookings = new BookingModell();

            EventModell test = new EventModell();
            if (bookings == null)
            {
                return NotFound();
            }

            test = GetEvent(bookings.Event_Id).Result;

            resBookings.Booking_Id = bookings.Booking_Id;
            resBookings.EventNamn = test.Event_Name;
            resBookings.Startdate = test.Event_Start_Datetime;
            resBookings.Enddate = test.Event_End_Datetime;
            resBookings.User_Name = bookings.User_Id.ToString();
            resBookings.User_Type = bookings.User_Type;


            
            return Ok(resBookings);
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

            var temp = db.Bookings.Where(e => e.Event_Id == bookings.Event_Id).Where(u => u.User_Id == bookings.User_Id);

            if (temp == null)
            {
                db.Entry(bookings).State = EntityState.Modified;
            }
            else
            {
                return BadRequest("Användaren finns redan inlaggd  på det eventet.");
            }
            
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

        private bool BookingsExists(int? id)
        {
            return db.Bookings.Count(e => e.Booking_Id == id) > 0;
        }
        private async Task<EventModell> GetEvent(int id)
        {
            EventModell eventList = new EventModell();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURLEvent);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = client.GetAsync("events/" + id).Result;

                if (res.IsSuccessStatusCode)
                {
                    var response = res.Content.ReadAsStringAsync().Result;
                    eventList = JsonConvert.DeserializeObject<EventModell>(response);
                    return eventList;
                }
            }
            
            return eventList;
        }
    }
}