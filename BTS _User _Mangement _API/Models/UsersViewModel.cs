using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTS__User__Mangement__API.Models
{
    public class UsersViewModel
    {
        public AdminAppDetails AdminAppDetails { get; set; }
        public KeyGenerator KeyGenerator { get; set; }
    }
}