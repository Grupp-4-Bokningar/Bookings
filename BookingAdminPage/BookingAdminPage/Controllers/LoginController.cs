
using BookingAdminPage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;

namespace BookingAdminPage.Controllers
{

    public class LoginController : Controller
    {
        string baseUrl = "http://193.10.202.76/";
        // GET: Login
        public async Task<bool> CheckLogin([Bind(Include = "Id,username,password,permission")] CredentialsRecived creds)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);

                    var response = client.PostAsJsonAsync("api/bookinglogin/", creds).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error try after some time.");
                    }
                }
            }
            return false;
        }
        public ActionResult Login()
        {
            ViewBag.LogginFailed = false;
            return View();
        }
        [HttpPost]
        public ActionResult Login(Credentials model, string ReturnUrl)
        {
            //buggfix om man öppnar Login/Login
            if (ReturnUrl == null)
            {
                ReturnUrl = "";
            }
            //if else om loginnen lyckas eller ej
            if (IsValid(model))
            {
                FormsAuthentication.SetAuthCookie(model.username, false);
                return Redirect(ReturnUrl);
            }
            else
            {
                ViewBag.LogginFailed = true;
                return View(model);
            }
        }
        private bool IsValid(Credentials model)
        {
            //ändra false till true om vi vill ha en permanent cookie
            //ska bara posta stupid
            CredentialsRecived cr = new CredentialsRecived();
            cr.Id = 0;
            cr.username = model.username;
            cr.password = model.password;
            cr.permission = "bookingadmin";
            return CheckLogin(cr).Result;
        }
    }
}