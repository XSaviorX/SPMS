using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DDNHRIS.Startup))]
namespace DDNHRIS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
