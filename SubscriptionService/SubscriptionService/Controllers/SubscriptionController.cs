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
using SubscriptionService.Models;

namespace SubscriptionService.Controllers
{
    public class SubscriptionController : ApiController
    {
        private SubscriptionModel db = new SubscriptionModel();

        // GET: api/Subscription
        public IQueryable<Subscriptions> GetSubscriptions()
        {
            return db.Subscriptions;
        }

        // GET: api/Subscription/5
        [ResponseType(typeof(Subscriptions))]
        public IHttpActionResult GetSubscriptions(int id)
        {
            Subscriptions subscriptions = db.Subscriptions.Find(id);
            if (subscriptions == null)
            {
                return NotFound();
            }

            return Ok(subscriptions);
        }

        // PUT: api/Subscription/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSubscriptions(int id, Subscriptions subscriptions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != subscriptions.Subscription_Id)
            {
                return BadRequest();
            }

            db.Entry(subscriptions).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionsExists(id))
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

        // POST: api/Subscription
        [ResponseType(typeof(Subscriptions))]
        public IHttpActionResult PostSubscriptions(Subscriptions subscriptions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Subscriptions.Add(subscriptions);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = subscriptions.Subscription_Id }, subscriptions);
        }

        // DELETE: api/Subscription/5
        [ResponseType(typeof(Subscriptions))]
        public IHttpActionResult DeleteSubscriptions(int id)
        {
            Subscriptions subscriptions = db.Subscriptions.Find(id);
            if (subscriptions == null)
            {
                return NotFound();
            }

            db.Subscriptions.Remove(subscriptions);
            db.SaveChanges();

            return Ok(subscriptions);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SubscriptionsExists(int id)
        {
            return db.Subscriptions.Count(e => e.Subscription_Id == id) > 0;
        }
    }
}