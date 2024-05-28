using MercadoPago.Resource.User;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Restaurant
{
    public class Booking
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("amountPeople")]
        public int AmountPeople { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("clientsId")]
        public string ClientsId { get; set; }

        [JsonProperty("businessName")]
        public string BusinessName { get; set; }

        [JsonProperty("stateClient")]
        public bool StateClient { get; set; }

        [JsonProperty("descriptionStateClient")]
        public string DescriptionStateClient { get; set; } = "";

        [JsonProperty("descriptionStateUser")]
        public string DescriptionStateUser { get; set; } = "";
    }
    public class BookingCreate
    {
        public int AmountPeople { get; set; }
        public DateTime DateReserve { get; set; }
        public string UserId { get; set; }
        public string ClientsId { get; set; }
        public string BusinessName { get; set; }
        public string DescriptionStateUser { get; set; } = "";
    }

    public class BookingConsult
    {
        public string UserId { get; set; }
        public string ClientsId { get; set; }
    }

    public class BookingConsulReserve
    {
        public string UserId { get; set; }
        public string ClientsId { get; set; }
        public DateTime DateReserve { get; set; }
    }
}
