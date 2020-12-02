using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BlogoSphere.Startup))]
namespace BlogoSphere
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
