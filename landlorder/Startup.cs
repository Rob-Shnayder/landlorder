using Microsoft.Owin;
using Owin;

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
