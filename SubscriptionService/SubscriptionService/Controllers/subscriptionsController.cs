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
        string baseURLEvent = "193.10.202.77/EventService/api/"; //event URL
        private SubscriptionModel db = new SubscriptionModel(); //Databas för prenumerationer.


        //GET: All for one user
        [Route("/user/{uid:int}")]
        public List<EventModell> GetSubscriptionsFromUser(int uid)
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
         
        private async Task<EventModell> GetEvent(int id) //Hämtar event från Event API. 
        {
            EventModell eventObj = new EventModell();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURLEvent); //193.10.202.77/EventService/api/
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = client.GetAsync("Events/" + id).Result; //lägg till när det finns ett api just för event

                if (res.IsSuccessStatusCode) //
                {
                    var respons = res.Content.ReadAsStringAsync().Result;
                    eventObj = JsonConvert.DeserializeObject<EventModell>(respons);
                    return eventObj;
                }
            }
            return eventObj;
        }

        // GET: api/subscriptions/5

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


