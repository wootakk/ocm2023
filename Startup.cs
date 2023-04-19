using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ISFMOCM_Project.Startup))]
namespace ISFMOCM_Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
