using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoud.Models.Profile
{
    public class LiveSearchUserVM
    {
        public String ID { get; set; }
        public String UserName { get; set; }

        public LiveSearchUserVM()
        {

        }

        public LiveSearchUserVM(ApplicationUser user)
        {
            ID = user.Id;
            UserName = user.UserName;
        }
    }
}