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
using System.Web.UI;
using BookingService;
using BookingService.Models;
using Newtonsoft.Json;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;
using NLog;

namespace BookingService.Controllers
{
    [RoutePrefix("api/Bookings")]//Grund urlen
    public class BookingsController : ApiController
    {
        public readonly Logger log = NLog.LogManager.GetCurrentClassLogger();
        private BookingModel db = new BookingModel();
        string baseURLEvent = "http://193.10.202.77/EventService/api/";
        string baseUrlLogin = "http://193.10.202.76/api/";
        // GET: api/Bookings
        [Route("")]
        public List<BookingModell> GetBookings()
        {
            List<BookingModell> bookingList = new List<BookingModell>();
            EventModell tempEvent = new EventModell();
            UserModel tempUser = new UserModel();

            var sql = db.Bookings.Select(s => s.Booking_Id).ToArray(); //Hämtar alla ID från alla rader.
            string userType;
            int eventId;
            for (var i = 0; i < sql.Count(); i++) //Kör forloopen så många gånger som rader det finns i databasen
            {
                var temp = sql[i];
                int userId = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Id).FirstOrDefault(); //Hämtar användar ID för första raden.

                try
                {
                    tempEvent = GetEvent(db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.Event_Id).FirstOrDefault()).Result; //Hämtar information från event tjänsten utefter eventID på raden.
                    tempUser = GetUser(userId).Result; //Hämtar användar ID från logintjänsten utefter användarID från databasen
                }
                catch(InvalidOperationException e) //Ifall tjänsterna inte funkar så hämtas enbart ID från databasen och returneras
                {
                    //Lägg in loggning här.
                    log.Error(e);
                    for(var t = 0; t < sql.Count(); t++)
                    {
                        temp = sql[t];
                        userType = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Type).FirstOrDefault();
                        userId = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Id).FirstOrDefault();
                        eventId = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.Event_Id).FirstOrDefault();
                        bookingList.Add(new BookingModell
                        {

                            Booking_Id = temp,
                            Event_Id = eventId,
                            User_Id = userId,
                            User_Type = userType
                        });
                    }
                    return bookingList;
                }

                userType = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Type).FirstOrDefault();

                if (tempUser != null)
                {
                    bookingList.Add(new BookingModell //Sparar ner alla värden i en lista
                    {
                        Booking_Id = temp,
                        Event_Id = tempEvent.Event_Id,
                        Event_Name = tempEvent.Event_Name,
                        Event_Start_Datetime = tempEvent.Event_Start_Datetime,
                        Event_End_Datetime = tempEvent.Event_End_Datetime,
                        User_Id = userId,
                        User_Name = tempUser.Firstname + " " + tempUser.Lastname,
                        User_Type = userType
                    });
                }
                else
                {
                    bookingList.Add(new BookingModell //Temporär felhantering då användaren inte finns i användardatabasen men i våran.
                    {
                        Booking_Id = temp,
                        Event_Id = tempEvent.Event_Id,
                        Event_Name = tempEvent.Event_Name,
                        Event_Start_Datetime = tempEvent.Event_Start_Datetime,
                        Event_End_Datetime = tempEvent.Event_End_Datetime,
                        User_Id = userId,
                        User_Name = userId.ToString(),
                        User_Type = userType
                    });
                }

            }
            return bookingList;
        }
        //Hämtar alla besökare på ett specifikt event
        [Route("Event/{eId:int}/Visitor")]//Hur urlen skall se ut
     
        public List<BookingModell> GetVisitorOnEvent(int eId)
        {
            List<BookingModell> bookingList = new List<BookingModell>();
            EventModell tempEvent = new EventModell();
            UserModel tempUser = new UserModel();

            var sql = db.Bookings.Where(b => b.User_Type == "Besökare").Where(s => s.Event_Id == eId).Select(s => s.Booking_Id).ToArray(); //Hämtar ID från alla rader som finns för alla besökare på ett specifikt event

            for(var i = 0; i < sql.Count(); i++) 
            {
                var temp = sql[i];
                int user = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Id).FirstOrDefault();
                string userType = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Type).FirstOrDefault();

                try
                {
                    tempEvent = GetEvent(db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.Event_Id).FirstOrDefault()).Result; //Hämtar  eventinfo
                    tempUser = GetUser(user).Result; //Hämtar användar info
                }
                catch (InvalidOperationException e)
                {
                    log.Error(e);
                    //Lägg in loggning här.
                    for (var t = 0; t < sql.Count(); t++)
                    {
                        temp = sql[t];
                        userType = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Type).FirstOrDefault();
                        user = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Id).FirstOrDefault();
                        bookingList.Add(new BookingModell
                        {

                            Booking_Id = temp,
                            Event_Id = eId,
                            User_Id = user,
                            User_Type = userType
                        });
                    }
                    return bookingList;
                }

                if (tempUser != null)
                {
                    bookingList.Add(new BookingModell
                    {
                        Booking_Id = temp,
                        Event_Id = tempEvent.Event_Id,
                        Event_Name = tempEvent.Event_Name,
                        Event_Start_Datetime = tempEvent.Event_Start_Datetime,
                        Event_End_Datetime = tempEvent.Event_End_Datetime,
                        User_Id = user,
                        User_Name = tempUser.Firstname + " " + tempUser.Lastname,
                        User_Type = userType
                    });
                }
                else
                {
                    bookingList.Add(new BookingModell
                    {
                        Booking_Id = temp,
                        Event_Id = tempEvent.Event_Id,
                        Event_Name = tempEvent.Event_Name,
                        Event_Start_Datetime = tempEvent.Event_Start_Datetime,
                        Event_End_Datetime = tempEvent.Event_End_Datetime,
                        User_Id = user,
                        User_Name = user.ToString(),
                        User_Type = userType
                    });
                }


            }

            return bookingList;
        }
        //Hämtar alla volontärer på ett specifikt event
        [Route("Event/{eId:int}/Volounteer")]

        public List<BookingModell> GetVolounteerOnEvent(int eId)
        {
            List<BookingModell> bookingList = new List<BookingModell>();
            EventModell tempEvent = new EventModell();
            UserModel tempUser = new UserModel();

            var sql = db.Bookings.Where(b => b.User_Type == "Volontär").Where(s => s.Event_Id == eId).Select(s => s.Booking_Id).ToArray(); //Hämtar ID från alla rader som finns för alla volontärer på ett specifikt event


            for (var i = 0; i < sql.Count(); i++)
            {
                var temp = sql[i];
                int user = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Id).FirstOrDefault();
                string userType = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Type).FirstOrDefault();

                try
                {
                    tempEvent = GetEvent(db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.Event_Id).FirstOrDefault()).Result; //Hämtar eventinfo
                    tempUser = GetUser(user).Result; //Hämtar användar info
                }
                catch (InvalidOperationException e)
                {
                    log.Error(e);
                    //Lägg in loggning här.
                    for (var t = 0; t < sql.Count(); t++)
                    {
                        temp = sql[t];
                        userType = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Type).FirstOrDefault();
                        user = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Id).FirstOrDefault();
                        bookingList.Add(new BookingModell
                        {

                            Booking_Id = temp,
                            Event_Id = eId,
                            User_Id = user,
                            User_Type = userType
                        });
                    }
                    return bookingList;
                }
                if (tempUser != null)
                {
                    bookingList.Add(new BookingModell
                    {
                        Booking_Id = temp,
                        Event_Id = tempEvent.Event_Id,
                        Event_Name = tempEvent.Event_Name,
                        Event_Start_Datetime = tempEvent.Event_Start_Datetime,
                        Event_End_Datetime = tempEvent.Event_End_Datetime,
                        User_Id = user,
                        User_Name = tempUser.Firstname + " " + tempUser.Lastname,
                        User_Type = userType
                    });
                }
                else
                {
                    bookingList.Add(new BookingModell
                    {
                        Booking_Id = temp,
                        Event_Id = tempEvent.Event_Id,
                        Event_Name = tempEvent.Event_Name,
                        Event_Start_Datetime = tempEvent.Event_Start_Datetime,
                        Event_End_Datetime = tempEvent.Event_End_Datetime,
                        User_Id = user,
                        User_Name = user.ToString(),
                        User_Type = userType
                    });
                }


            }

            return bookingList;
        }

        //Ska hämta all info om en besökare/volontär :)
        [Route("User/{uId:int}")]

        public List<BookingModell> GetBookingFromUser(int uId)
        {
            List<BookingModell> bookingList = new List<BookingModell>();
            EventModell tempEvent = new EventModell();
            UserModel tempUser = new UserModel();

            var sql = db.Bookings.Where(u => u.User_Id == uId).Select(u => u.Booking_Id).ToArray(); //Hämtar bokningsID utefter en specifik användare
            int eventId;
            for(int i = 0; i<sql.Count(); i++)
            {
                int temp = sql[i];

                string userType = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Type).FirstOrDefault();

                try
                {
                    tempEvent = GetEvent(db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.Event_Id).FirstOrDefault()).Result; //Hämtar evendata
                    tempUser = GetUser(uId).Result; //Hämta användar data.
                }
                catch (InvalidOperationException e)
                {
                    log.Error(e);
                    //Lägg in loggning här.
                    for (var t = 0; t < sql.Count(); t++)
                    {
                        temp = sql[t];
                        userType = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.User_Type).FirstOrDefault();
                        eventId = db.Bookings.Where(s => s.Booking_Id == temp).Select(s => s.Event_Id).FirstOrDefault();
                        bookingList.Add(new BookingModell
                        {

                            Booking_Id = temp,
                            Event_Id = eventId,
                            User_Id = uId,
                            User_Type = userType
                        });
                    }
                    return bookingList;
                }
                if (tempUser != null)
                {
                    bookingList.Add(new BookingModell
                    {
                        Booking_Id = temp,
                        Event_Id = tempEvent.Event_Id,
                        Event_Name = tempEvent.Event_Name,
                        Event_Start_Datetime = tempEvent.Event_Start_Datetime,
                        Event_End_Datetime = tempEvent.Event_End_Datetime,
                        User_Id = uId,
                        User_Name = tempUser.Firstname + " " + tempUser.Lastname,
                        User_Type = userType
                    });
                }
                else
                {
                    bookingList.Add(new BookingModell
                    {
                        Booking_Id = temp,
                        Event_Id = tempEvent.Event_Id,
                        Event_Name = tempEvent.Event_Name,
                        Event_Start_Datetime = tempEvent.Event_Start_Datetime,
                        Event_End_Datetime = tempEvent.Event_End_Datetime,
                        User_Id = uId,
                        User_Name = uId.ToString(),
                        User_Type = userType
                    });
                }
            }


            return bookingList;

        }


        // GET: api/Bookings/5
        [Route("{id:int}")]
        [ResponseType(typeof(BookingModell))]
        public IHttpActionResult GetBookings(int id)
        {
            Bookings bookings = db.Bookings.Find(id);
            BookingModell resBookings = new BookingModell();
            UserModel tempUser = new UserModel();
            EventModell test = new EventModell();

            if (bookings == null)
            {
                return NotFound();
            }

            try
            {
                test = GetEvent(bookings.Event_Id).Result;
                tempUser = GetUser(bookings.User_Id).Result;
            }
            catch(InvalidOperationException e)
            {

            }
            

            resBookings.Booking_Id = bookings.Booking_Id;
            resBookings.Event_Id = test.Event_Id;
            resBookings.Event_Name = test.Event_Name;
            resBookings.Event_Start_Datetime = test.Event_Start_Datetime;
            resBookings.Event_End_Datetime = test.Event_End_Datetime;
            resBookings.User_Id = bookings.User_Id;
            if(tempUser != null)
            {
                resBookings.User_Name = tempUser.Firstname + " " + tempUser.Lastname;
            }
            else
            {
                resBookings.User_Name = bookings.User_Id.ToString();
            }
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
                log.Error("Fel parametrar inskickade, BadRequest");
                return BadRequest(ModelState);
            }

            if (id != bookings.Booking_Id)
            {
                log.Error("Inskickade ID finns inte");
                return BadRequest();
            }

            //Felhantering för dubbellagring

            var temp = db.Bookings.Where(s => s.Event_Id == bookings.Event_Id).Where(s => s.User_Id == bookings.User_Id)
                            .Where(s => s.Booking_Id != bookings.Booking_Id).Select(s => s.Booking_Id).ToArray();

            if(temp.Count() < 1)
            {
                try
                {
                    db.Entry(bookings).State = EntityState.Modified;
                    db.SaveChanges();
                    log.Info("Lyckades uppdatera");
                }
                catch (DbUpdateConcurrencyException e)
                {
                    log.Error(e);
                    if (!BookingsExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
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
                    log.Error("Fel parametrar inskickade, BadRequest");
                    return BadRequest(ModelState);
                }

                
                //Event id & userID får inte finnas någon annan stans.
                var temp = db.Bookings.Where(s => s.Event_Id == bookings.Event_Id).Where(s => s.User_Id == bookings.User_Id)
                            .Where(s => s.Booking_Id != bookings.Booking_Id).Select(s => s.Booking_Id).ToArray();

                if(temp.Count() < 1)
                {
                    db.Bookings.Add(bookings);
                    db.SaveChanges();
                    log.Info("Lyckdes skapa ny bokning");
                    return Ok(bookings);
                }
                else
                {
                    return BadRequest("Användaren finns redan på eventet");
                }

                
                
                
            }
            catch(Exception e)
            {
                log.Error(e);
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
            log.Info("Lyckades ta bort bokning");
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
            log.Info("Tog bort bokning för specifik användare och event");
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
        private async Task<EventModell> GetEvent(int id) //Hämtar ett event från eventjänsten
        {
            EventModell eventObj = new EventModell();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURLEvent);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = client.GetAsync("events/" + id).Result;

                if (res.IsSuccessStatusCode)
                {
                    var response = res.Content.ReadAsStringAsync().Result;
                    eventObj = JsonConvert.DeserializeObject<EventModell>(response);
                    return eventObj;
                }
            }
            
            return eventObj;
        }
        private async Task<UserModel> GetUser(int id) //Hämtar användare från logintjänsten
        {
            UserModel userObj = new UserModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrlLogin);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = client.GetAsync("visitor/" + id).Result;

                if (res.IsSuccessStatusCode)
                {
                    var response = res.Content.ReadAsStringAsync().Result;
                    userObj = JsonConvert.DeserializeObject<UserModel>(response);
                    return userObj;
                }

                return userObj;
            }
        }
    }
}