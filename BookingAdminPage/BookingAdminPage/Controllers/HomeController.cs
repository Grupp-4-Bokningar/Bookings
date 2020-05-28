using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookingAdminPage.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;

namespace BookingAdminPage.Controllers
{
    public class HomeController : Controller
    {
        string baseUrl = "http://193.10.202.81/BookingService/";
        // GET: Home
        [Authorize]
        public async Task<ActionResult> Index()
        {


            List<AdminDataModell> allBookingList = new List<AdminDataModell>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(baseUrl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/bookings");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var responseBookings = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    allBookingList = JsonConvert.DeserializeObject<List<AdminDataModell>>(responseBookings);

                }
                //returning the employee list to view
                return View(allBookingList);
            }

        }
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Event_Id,User_Id,User_Type")] BookingModel booking)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);

                    var response = await client.PostAsJsonAsync("api/Bookings", booking);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error try after some time.");
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<ActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingModel booking = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                HttpResponseMessage Res = await client.GetAsync($"api/bookings/{id}");

                if (Res.IsSuccessStatusCode)
                {
                    var responseBooking = Res.Content.ReadAsStringAsync().Result;
                    //booking = await Res.Content.ReadAsAsync<AdminDataModell>();
                    booking = JsonConvert.DeserializeObject<BookingModel>(responseBooking);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error try after some time.");
                }
            }
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }
        [HttpPost]
        public async Task<ActionResult> Update([Bind(Include = "Booking_Id,Event_Id,User_Id,User_Type")] BookingModel booking)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    var response = await client.PutAsJsonAsync($"api/Bookings/{booking.Booking_Id}", booking);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error try after some time.");
                    }
                }
                return RedirectToAction("Index");
            }
            return View(booking);
        }
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BookingModel booking = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                var result = await client.GetAsync($"api/Bookings/{id}");

                if (result.IsSuccessStatusCode)
                {
                    booking = await result.Content.ReadAsAsync<BookingModel>();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error try after some time.");
                }
            }

            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);

                var response = await client.DeleteAsync($"api/Bookings/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                    ModelState.AddModelError(string.Empty, "Server error try after some time.");
            }
            return RedirectToAction("Index");
        }
    }
}