using Android.Gms.Maps.Model;
using Android.Webkit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Restaurant
{
    public  class MenuResponse
    {
        [JsonProperty("categories")]
        public List<CategoriesRestaurantResponse> Categories { get; set; } = new List<CategoriesRestaurantResponse>();

        [JsonProperty("drinks")]
        public List<DrinksResponse> Drinks { get; set; } = new List<DrinksResponse>();
        [JsonProperty("dishes")]
        public List<DishesResponse> Dishes { get; set; } = new List<DishesResponse>();
    }
}
