using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Login
{
    public class CreateUserRequest
    {
        public string name { get; set; }
        public string email { get; set; }
        public string pass { get; set; }    
        public string userName { get; set; }
    }
}
