using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetwork.Models.ViewModels.Account
{
    public class LoginVM
    {
        [Rqeuired]
        public string Username{ get; set; }

        [Rqeuired]
        public string Password { get; set; }
    }
}