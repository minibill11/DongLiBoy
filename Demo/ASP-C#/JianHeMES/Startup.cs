using Microsoft.Owin;
using Owin;
using JianHeMES;

[assembly: OwinStartupAttribute(typeof(JianHeMES.Startup))]
namespace JianHeMES
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
