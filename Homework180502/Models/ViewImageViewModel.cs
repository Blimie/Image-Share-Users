using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Homework180502.Data;

namespace Homework180502.Models
{
    public class ViewImageViewModel
    {
        public bool HasPermissionToView { get; set; }
        public Image Image { get; set; }
        public string Message { get; set; }
    }
    public class HomePageViewModel
    {
        public User User { get; set; }
        public string Message { get; set; }
    }
    public class LoginViewModel
    {
        public string Message { get; set; }
    }
    public class MyAccountViewModel
    {
        public IEnumerable<Image> Images { get; set; }
    }

}