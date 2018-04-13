using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ChatLoud.Startup))]
namespace ChatLoud
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
