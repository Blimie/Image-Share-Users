using Homework180502.Data;
using Homework180502.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Homework180502.Controllers
{
    public class UserController : Controller
    {
        ImageLoginDB db = new ImageLoginDB(Properties.Settings.Default.ConStr);
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(User user, string password)
        {
            db.AddUser(user, password);
            return RedirectToAction("Index", "Images");
        }
        public ActionResult Login()
        {
            var viewModel = new LoginViewModel();
            if (TempData["Message"] != null)
            {
                viewModel.Message = (string)TempData["Message"];
            }
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            User user = db.Login(email, password);
            if (user == null)
            {
                TempData["Message"] = $"Password entered is incorrect, please try again.";
                return RedirectToAction("Login");
            }
            FormsAuthentication.SetAuthCookie(email, true);
            return RedirectToAction("Index", "Images");
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Images");
        }
        [Authorize]
        public ActionResult MyAccount()
        {  
            User user = db.GetByEmail(User.Identity.Name);
            return View(new MyAccountViewModel
            {
                Images = db.GetImagesByUserId(user.Id)
            });
        }
    }
}