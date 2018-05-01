using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(tutioncloud.Startup))]
namespace tutioncloud
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
