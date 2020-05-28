
using BookingAdminPage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BookingAdminPage.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Credentials model, string ReturnUrl)
        {
            //if else om loginnen lyckas eller ej
            if (IsValid(model))
            {
                //ändra false till true om vi vill ha en permanent cookie
                FormsAuthentication.SetAuthCookie(model.username, false);
                return Redirect(ReturnUrl);
            }
            else
            {
                return View(model);
            }
        }
        private bool IsValid(Credentials model)
        {
            return (model.username == "test" && model.password == "test");
        }
    }
}