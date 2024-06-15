using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Login
{
    public class LoginRequest
    {
        public string userName { get; set; }
        public string userPassword { get; set; }
    }
    public class LogoutRequest
    {
        public string userId { get; set; }
    }
    public class ChangePassRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordOld { get; set; }
        public string PasswordNew { get; set; }
    }   

}
