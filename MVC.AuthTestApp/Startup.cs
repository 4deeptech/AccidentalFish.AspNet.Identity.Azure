using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC.AuthTestApp.Startup))]
namespace MVC.AuthTestApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
