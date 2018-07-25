using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ShoesStore_agforl.Startup))]
namespace ShoesStore_agforl
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
