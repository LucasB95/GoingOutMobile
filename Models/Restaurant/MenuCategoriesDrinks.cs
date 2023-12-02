using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Restaurant
{
    public class MenuCategoriesDrinks : List<DrinksResponse>
    {
        public int IdCategory { get; set; }
        public string NameCategory { get; set; }

        public MenuCategoriesDrinks(int idCategory, string nameCategory, List<DrinksResponse> drinks) : base(drinks)
        {

            IdCategory = idCategory;
            NameCategory = nameCategory;
        }

    }
}
