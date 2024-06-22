using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Login
{
    public class RecoveryPassword
    {
        public Guid Id { get; set; }
        public DateTime DateChange { get; set; }
        public string Password { get; set; }
        public int CodeGenerate { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}