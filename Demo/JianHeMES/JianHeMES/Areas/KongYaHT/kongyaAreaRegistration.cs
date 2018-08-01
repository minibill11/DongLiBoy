using System.Web.Mvc;

namespace JianHeMES.Areas.kongya
{
    public class kongyaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "KongYaHT";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "KongYaHT",
                "KongYaHT/{controller}/{action}/{id}",
                new { action = "KongYaIndex", id = UrlParameter.Optional }
            );
        }
    }
}