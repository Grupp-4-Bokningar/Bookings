using BookingAdminPage.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace BookingAdminPage.Controllers
{
    public class EventPeopleController : Controller
    {
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
                    //returning the employee list to view  
                    return View(EventInfo);
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
                            
                        }
                    }

                   

                    //returning the employee list to view  
                    return View(EventInfo);
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
                    HttpResponseMessage Res = await client.GetAsync("api/Bookings/Event/" + id + "/volounteer");


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
    }
}