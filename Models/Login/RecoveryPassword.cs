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
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("dateRecover")]
        public DateTime DateRecover { get; set; }

        [JsonProperty("codeGenerate")]
        public int CodeGenerate { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
