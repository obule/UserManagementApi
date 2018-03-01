using System.Web;
using System.Web.Mvc;

namespace BTS__User__Mangement__API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
