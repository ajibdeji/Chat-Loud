using ChatLoud.Models.Profile;
using ChatLoud.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatLoud.Controllers
{
    public class ProfileController : Controller
    {
        private ServiceReference1.Service1Client client = new ServiceReference1.Service1Client("WSHttpBinding_IService1");
        // GET: Profile/{id}
        public ActionResult Index(string id)
        {
            UserProfile userProfile = client.GetUserProfile(id);
            var viewModel = new ProfileVM()
            {
                Profile = userProfile
            };
            return View(viewModel);
        }

        
        //Post: profile/LiveSearch
        [HttpPost]
        public JsonResult LiveSearch(string searchVal)
        {
            List<UserProfile> profiles = null;
            try
            {
                profiles = client.GetSearchedUsers(searchVal).ToList();
            }
            catch (Exception ex)
            {
                string smg = ex.Message;
            }
            return Json(profiles);
        }
          
         
    }
}