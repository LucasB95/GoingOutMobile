using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Login
{
    public class UserResponse
    {
        public string id { get; set; }
        public string tokenGoingOut { get; set; }
        public string message { get; set; }
    }
    public class TokenModel
    {
        public string? Token { get; set; }
    }


}
