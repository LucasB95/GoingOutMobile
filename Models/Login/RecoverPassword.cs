using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Login
{
    public class RecoverPassword
    {
        public string Email { get; set; }
        public string UserName { get; set; }
    }
    public class RecoverPasswordResponse
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("dateRecover")]
        public DateTime DateRecover { get; set; }

        [JsonProperty("codeGenerate")]
        public string CodeGenerate { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
