using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CheckMailAddress.Startup))]
namespace CheckMailAddress
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
