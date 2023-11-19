using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Restaurant
{
    public class RestaurantResponse
    {
        [JsonProperty("businessName")]
        public string BusinessName { get; set; }
        [JsonProperty("adress")]
        public AdressResponse Adress { get; set; }
    }
}
