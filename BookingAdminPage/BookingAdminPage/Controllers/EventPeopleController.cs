using BookingAdminPage.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;
using NLog;

namespace BookingAdminPage.Controllers
{
    public class EventPeopleController : Controller
    {
        public readonly Logger logger = NLog.LogManager.GetCurrentClassLogger();
        // GET: EventPeople
        string baseUrl = "http://193.10.202.81/BookingService/";
        string baseUrlEvent = "http://193.10.202.77/EventService/";
        string baseUrlLogin = "http://193.10.202.76/";
        [Authorize]
        public async Task<ActionResult> Index()
        {


            List<User> userList = new List<User>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(baseUrlLogin);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/visitor");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var response = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    userList = JsonConvert.DeserializeObject<List<User>>(response);

                }
                //returning the employee list to view
                return View(userList);
            }

        }
        [Authorize]
        public async Task<ActionResult> People(int id)
        {
            try
            {
                List<AdminDataModell> EventInfo = new List<AdminDataModell>();
                AdminDataModell tempObj = new AdminDataModell();

                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(baseUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    HttpResponseMessage Res = await client.GetAsync("api/Bookings/User/"+id);

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var AdminDataModellResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        EventInfo = JsonConvert.DeserializeObject<List<AdminDataModell>>(AdminDataModellResponse);

                    }
                    if(EventInfo.Count > 0)
                    {
                        //returning the employee list to view  
                        return View(EventInfo);
                    }
                    else
                    {
                        tempObj.User_Id = id;
                        EventInfo.Add(tempObj);
                        return View(EventInfo);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Hoppsan! här blev det fel.");
                throw;
            }

        }
        [Authorize]
        public async Task<ActionResult> SpecificEvent(int id)
        {
            try
            {
                List<AdminDataModell> EventInfo = new List<AdminDataModell>();
                List<AdminDataModell> tempList = new List<AdminDataModell>();
                AdminDataModell tempObj = new AdminDataModell();

                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(baseUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    HttpResponseMessage Res = await client.GetAsync("api/Bookings/Event/" +id +"/visitor");
                    

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var AdminDataModellResponse = Res.Content.ReadAsStringAsync().Result;
                        
                        //Deserializing the response recieved from web api and storing into the Employee list  
                        EventInfo = JsonConvert.DeserializeObject<List<AdminDataModell>>(AdminDataModellResponse);


                        tempList = getVolounteer(id).Result;

                        if(tempList != null)
                        {
                            EventInfo.AddRange(tempList);
                        }
                    }

                   if(EventInfo.Count > 0)
                    {
                        return View(EventInfo);
                    }
                    else
                    {
                        tempObj.Event_Id = id;
                        EventInfo.Add(tempObj);
                        return View(EventInfo);
                    }

                    //returning the employee list to view  
                    
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Hoppsan! här blev det fel.");
                throw;
            }

        }
        public async Task<List<AdminDataModell>> getVolounteer(int id)
        {
            try
            {
                List<AdminDataModell> EventInfo = new List<AdminDataModell>();

                using (var client = new HttpClient())
                {
                    //Passing service base url  
                    client.BaseAddress = new Uri(baseUrl);

                    client.DefaultRequestHeaders.Clear();
                    //Define request data format  
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                    HttpResponseMessage Res = client.GetAsync("api/Bookings/Event/" + id + "/volounteer").Result;


                    //Checking the response is successful or not which is sent using HttpClient  
                    if (Res.IsSuccessStatusCode)
                    {
                        //Storing the response details recieved from web api   
                        var AdminDataModellResponse = Res.Content.ReadAsStringAsync().Result;

                        //Deserializing the response recieved from web api and storing into the Employee list  
                        EventInfo = JsonConvert.DeserializeObject<List<AdminDataModell>>(AdminDataModellResponse);

                    }



                    //returning the employee list to view  
                    return EventInfo;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Hoppsan! här blev det fel.");
                throw;
            }
        }
        [Authorize]
        public async Task<ActionResult> Event()
        {
            List<EventModell> allEventsList = new List<EventModell>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(baseUrlEvent);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Events");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var responseEvent = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    allEventsList = JsonConvert.DeserializeObject<List<EventModell>>(responseEvent);

                }
                //returning the employee list to view
                return View(allEventsList);
            }
        }
        [Authorize]
        public async Task<ActionResult> EditEvent(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
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
        public async Task<ActionResult> UpdateEvent([Bind(Include = "Booking_Id,Event_Id,User_Id,User_Type")] BookingModel booking)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    var response = await client.PutAsJsonAsync($"api/Bookings/{booking.Booking_Id}", booking);
                    if (response.IsSuccessStatusCode)
                    {
                        logger.Info("Uppdatera baserat på parametrar");
                        return RedirectToAction("Event");
                    }
                    else
                    {
                        logger.Error("Misslyckades med uppdatering");
                        ModelState.AddModelError(string.Empty, "Server error try after some time.");
                    }
                }
                return RedirectToAction("Event");
            }
            return View(booking);
        }
        [Authorize]
        public async Task<ActionResult> DeleteEvent(int id)
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
                    logger.Info("Raderade bokning baserat på parametrar");
                    return RedirectToAction("Event");
                }
                else
                    logger.Error("Misslyckades med att radera");
                    ModelState.AddModelError(string.Empty, "Server error try after some time.");
            }
            return RedirectToAction("Event");
        }
        [Authorize]
        public ActionResult CreateEvent(int id)
        {
            
            BookingModel booking = new BookingModel();
            booking.Event_Id = id;

             return View(booking);
           
        }
        [Authorize]
        public ActionResult CreateUser(int id)
        {

            BookingModel booking = new BookingModel();
            booking.User_Id = id;

            return View(booking);

        }
        [HttpPost]
        public async Task<ActionResult> CreateEvent([Bind(Include = "Event_Id,User_Id,User_Type")] BookingModel booking)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);

                    var response = await client.PostAsJsonAsync("api/Bookings", booking);
                    if (response.IsSuccessStatusCode)
                    {
                        logger.Info("Skapade ny bokning baserat på parametrar");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        logger.Error("Misslyckades med att skapa nytt");
                        ModelState.AddModelError(string.Empty, "Server error try after some time.");
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}