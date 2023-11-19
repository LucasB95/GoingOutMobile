using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Config;
public class Settings
{
    public string UrlBase { get; set; }
    public string SecretKey { get; set; }
    public string DbKey { get; set; }

    public string UrlBaseRestaurant { get; set; }
    public string SecretKeyRestaurant { get; set; }
    public string DbKeyRestaurant { get; set; }
}

