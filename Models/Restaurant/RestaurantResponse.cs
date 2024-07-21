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

        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("isBookmarkEnabled")]
        public bool IsBookmarkEnabled { get; set; }

        [JsonProperty("adress")]
        public AdressResponse Adress { get; set; }

    }
    public class ClientsList
    {
        [JsonProperty("clientsResponse")]
        public IEnumerable<RestaurantResponse> ClientsResponse { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("elementPage")]
        public int ElementPage { get; set; }

        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("prevPage")]
        public bool PrevPage { get; set; }

        [JsonProperty("nextPage")]
        public bool NextPage { get; set; }
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
