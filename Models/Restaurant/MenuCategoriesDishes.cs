using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Restaurant
{
    public class MenuCategoriesDishes : List<DishesResponse>
    {
        public int IdCategory { get; set; }
        public string NameCategory { get; set; }

        public MenuCategoriesDishes(int idCategory, string nameCategory, List<DishesResponse> dishes) : base(dishes)
        {

            IdCategory = idCategory;
            NameCategory = nameCategory;
        }

    }
}
