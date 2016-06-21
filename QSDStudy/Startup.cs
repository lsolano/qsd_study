using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QSDStudy.Startup))]
namespace QSDStudy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
