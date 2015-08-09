using Microsoft.Owin;
using Owin;
using landlorder;

[assembly: OwinStartupAttribute(typeof(landlorder.Startup))]
namespace landlorder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
