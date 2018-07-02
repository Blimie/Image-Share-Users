using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Homework180502.Data;
using Homework180502.Models;
using System.IO;

namespace Homework180502.Controllers
{
    public class ImagesController : Controller
    {
        ImageLoginDB db = new ImageLoginDB(Properties.Settings.Default.ConStr);
        [Authorize]
        public ActionResult Index()
        {
            HomePageViewModel viewModel = new HomePageViewModel
            {
                User = db.GetByEmail(User.Identity.Name)
            };
            if (TempData["message"] != null)
            {
                viewModel.Message = (string)TempData["message"];
            }
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Upload(Image image, HttpPostedFileBase imageFile)
        {
            if (image.Password == null || imageFile == null)
            {
                TempData["message"] = "Please choose an image to upload and enter a password";
                return Redirect("/");
            }
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            imageFile.SaveAs(Path.Combine(Server.MapPath("/UploadedImages"), fileName));
            image.FileName = fileName;
            db.AddImage(image);
            return View(image);
        }
        [Authorize]
        [HttpPost]
        public ActionResult DeleteImage(int imageId)
        {
            db.DeleteImage(imageId);
            return RedirectToAction("MyAccount", "User");
        }
        public ActionResult ViewImage(int id)
        {
            var viewModel = new ViewImageViewModel();
            if (TempData["message"] != null)
            {
                viewModel.Message = (string)TempData["message"];
            }

            if (!HasPermissionToView(id))
            {
                viewModel.HasPermissionToView = false;
                viewModel.Image = new Image { Id = id };
            }
            else
            {
                viewModel.HasPermissionToView = true;
                db.UpdateViewsCount(id);
                var image = db.GetImage(id);
                if (image == null)
                {
                    return RedirectToAction("Index");
                }
                viewModel.Image = image;
            }
            return View(viewModel);
        }
        private bool HasPermissionToView(int id)
        {
            if (Session["allowedids"] == null)
            {
                return false;
            }

            var allowedIds = (List<int>)Session["allowedids"];
            return allowedIds.Contains(id);
        }
        [HttpPost]
        public ActionResult ViewImage(int id, string password)
        {
            var image = db.GetImage(id);
            if (image == null)
            {
                return RedirectToAction("Index");
            }
            if (password != image.Password)
            {
                TempData["message"] = "Invalid password. Please try again.";
            }
            else
            {
                List<int> allowedIds;
                if (Session["allowedids"] == null)
                {
                    allowedIds = new List<int>();
                    Session["allowedids"] = allowedIds;
                }
                else
                {
                    allowedIds = (List<int>)Session["allowedids"];
                }

                allowedIds.Add(id);
            }
            return Redirect($"/images/viewimage?id={id}");
        }
    }
}