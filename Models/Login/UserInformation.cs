using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Login
{
    public class UserInformation
    {
        public string UserId { get; set; }
        public string UserProfileId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTime? LockoutEnd { get; set; }
    }
}
