using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatLoud.Controllers
{
    public class HomeController : Controller
    {
        private ServiceReference1.Service1Client client = new ServiceReference1.Service1Client("WSHttpBinding_IService1");

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.OnlineCount = client.GetOnlineUsers().ToList().Count;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}