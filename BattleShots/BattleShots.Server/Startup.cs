using System;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(BattleShots.Server.Startup))]

namespace BattleShots.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}