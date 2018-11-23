using JianHeMES.Controllers;
using System.Web;
using System.Web.Mvc;

namespace JianHeMES
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
