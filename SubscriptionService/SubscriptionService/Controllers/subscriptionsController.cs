using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SubscriptionService.Models;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Newtonsoft.Json;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;

namespace SubscriptionService.Controllers
{
    [RoutePrefix("api/subscriptions")]  //standard route, kommer alltid före vanlig route. 
    public class subscriptionsController : ApiController
    {
        string baseURLEvent = "http://193.10.202.77/EventService/";
        private SubscriptionModel db = new SubscriptionModel();


        //GET: All for one user
        [Route("user/{uid:int}")]
        public List<EventModell> GetEventSubscriptionsFromUser(int uid)
        {
            List<EventModell> eventList = new List<EventModell>(); //lista med event


            var sql = db.subscriptions.Where(u => u.user_Id == uid).Select(u => u.subscription_Id).ToArray(); //hämtar id från databas där user_Id=uid från inparameter

            for (var i = 0; i < sql.Count(); i++) //loopar igenom för att lägga till 
            {
                var temp = sql[i];

                try
                {
                    eventList.Add(GetEvent(db.subscriptions.Where(s => s.subscription_Id == temp).Select(s => s.event_location_Id).FirstOrDefault()).Result); //andropar GetEvent som hämtar event från eventAPI
                }
                catch (InvalidOperationException e)
                {
                    //TODO Lägg in loggning här.
                    Console.Write(e); //loggar exception så länge. 
                }
            }

            return eventList; //returnerar lista med event där det endast finns event som är kopplad till användare.
        }
        [Route("subscription/{uid:int}")]
        public List<subscription> GetSubscriptionsFromUser(int uid)
        {
            List<subscription> subsList = new List<subscription>();


            var sql = db.subscriptions.Where(u => u.user_Id == uid).Select(u => u.subscription_Id).ToArray();

            for (var i = 0; i < sql.Count(); i++)
            {
                var temp = sql[i];
                int User_Id = db.subscriptions.Where(u => u.subscription_Id == temp).Select(u => u.user_Id).FirstOrDefault();
                int Event_Location_Id = db.subscriptions.Where(u => u.subscription_Id == temp).Select(u => u.event_location_Id).FirstOrDefault();
                try
                {
                    subsList.Add(new subscription {
                        subscription_Id = temp,
                        user_Id = User_Id,
                        event_location_Id = Event_Location_Id
                    });
                }
                catch (InvalidOperationException e)
                {
                    //Lägg in loggning här.
                    Console.Write(e);
                }
            }

            return subsList; //returnerar lista med event där det endast finns event som är kopplad till användare.
        }

        private async Task <List<EventModell>> GetEvent(int id)
        {
            List<EventModell> eventObj = new List <EventModell>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURLEvent); //193.10.202.77/EventService/api/
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = client.GetAsync("api/Events/Place/" + id).Result; //lägg till när det finns ett api just för event

                if (res.IsSuccessStatusCode) //
                {
                    var respons = res.Content.ReadAsStringAsync().Result;
                    eventObj = JsonConvert.DeserializeObject<List<EventModell>>(respons);
                    
                    return eventObj;
                }
            }
            return eventObj;
        }

        // GET: api/subscriptions/5
        [Route("/{id:int}")]
        [ResponseType(typeof(subscription))]
        public IHttpActionResult Getsubscription(int id) //Hämtar prenumerattioner med id
        {
            subscription subscription = db.subscriptions.Find(id);
            if (subscription == null)
            {
                return NotFound();
            }

            return Ok(subscription);
        }

        // PUT: api/subscriptions/5
        [Route("/{id:int}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putsubscription(int id, subscription subscription)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != subscription.subscription_Id)
            {
                return BadRequest();
            }

            db.Entry(subscription).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!subscriptionExists(id))
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

        // POST: api/subscriptions
        [Route("")]
        [ResponseType(typeof(subscription))]
        public IHttpActionResult Postsubscription(subscription subscription)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.subscriptions.Add(subscription);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = subscription.subscription_Id }, subscription);
        }

        // DELETE: api/subscriptions/5
        [Route("/delete/{uid:int}")]  //tar bort ett event
        public IHttpActionResult Deletesubscription(int id)
        {
            subscription subscription = db.subscriptions.Find(id);
            if (subscription == null)
            {
                return NotFound();
            }

            db.subscriptions.Remove(subscription);
            db.SaveChanges();

            return Ok(subscription);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool subscriptionExists(int id)
        {
            return db.subscriptions.Count(e => e.subscription_Id == id) > 0;
        }
    }
}


