﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoingOutMobile.Services.Interfaces;

namespace GoingOutMobile.Services
{
    public class NavegacionService : INavegacionService
    {
        public Task GoToAsync(string uri)
        {
            return Shell.Current.GoToAsync(uri);
        }

        public Task GoToAsync(string uri, IDictionary<string, object> parameters)
        {
            return Shell.Current.GoToAsync(uri, parameters);
        }
    }
}