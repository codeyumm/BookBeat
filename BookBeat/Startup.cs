using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookBeat.Startup))]
namespace BookBeat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
