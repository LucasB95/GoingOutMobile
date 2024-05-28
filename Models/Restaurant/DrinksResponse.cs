using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace GoingOutMobile.Models.Restaurant
{
    public class DrinksResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        [MaxLength(150)]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("categories")]
        public CategoriesRestaurantResponse Categories { get; set; }
    }

}
