using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Restaurant
{
    public class AdressResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("province")]
        public string Province { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("numeration")]
        public int Numeration { get; set; }
        [JsonProperty("pc")]
        public int PC { get; set; }
    }
}
