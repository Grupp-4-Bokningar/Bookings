using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AdminPrototypTest.Models;
using Newtonsoft.Json;
namespace AdminPrototypTest.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        string baseURL = "193.10.202.81/";
        public async Task<ActionResult> Index()
        {

            List<BookingsModel> bokningarList = new List<BookingsModel>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURL);

                    client.DefaultRequestHeaders.Clear();

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage res = await client.GetAsync("/api/Bookings");

                    if (res.IsSuccessStatusCode)
                    {
                        var bookingsResponse = res.Content.ReadAsStringAsync().Result;

                        bokningarList = JsonConvert.DeserializeObject<List<BookingsModel>>(bookingsResponse);
                    }
                }

                return View(bokningarList);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Något gick fel vid anrop mot tjänsten.");
                return View();
            }

        }
        public async Task<ActionResult> Detaljer(int? id)
        {

            BookingsModel bokningar = new BookingsModel();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURL);

                    client.DefaultRequestHeaders.Clear();

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage res = await client.GetAsync("/api/Bookings/" + id);

                    if (res.IsSuccessStatusCode)
                    {
                        var bookingsResponse = res.Content.ReadAsStringAsync().Result;

                        bokningar = JsonConvert.DeserializeObject<BookingsModel>(bookingsResponse);
                    }
                }

                return View(bokningar);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("Något gick fel vid anrop mot tjänsten.");
                return View();
            }

        }
    }
}