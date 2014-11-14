using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Schols.Models
{
    public class UserModel
    {
        public string username { get; set; }
        public string userpassword { get; set; }
        public string accesstoken { get; set; }
        public string refreshtoken { get; set; }
        public string fullname { get; set; }
        public string usermajor { get; set; }
    }
}