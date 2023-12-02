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
        [JsonProperty("idClient")]
        public string IdClient { get; set; }

        [JsonProperty("businessName")]
        public string BusinessName { get; set; }

        [JsonProperty("isBookmarkEnabled")]
        public bool IsBookmarkEnabled { get; set; }

        [JsonProperty("adress")]
        public AdressResponse Adress { get; set; }

    }
    public class RestaurantResponseFavorite
    {
        [JsonProperty("idClient")]
        public string IdClient { get; set; }

        [JsonProperty("businessName")]
        public string BusinessName { get; set; }

        [JsonProperty("adress")]
        public AdressResponse Adress { get; set; }

        [JsonProperty("adress")]
        public bool IsBookmarkEnabled { get; set; }
    }
}
