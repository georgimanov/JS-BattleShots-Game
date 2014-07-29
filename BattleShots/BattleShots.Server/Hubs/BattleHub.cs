using System;
using Microsoft.AspNet.SignalR;

namespace BattleShots.Server.Hubs
{
    public class BattleHub : Hub
    {
        public void InformPlayerMoved(string msg)
        {
            Clients.Caller.hello(msg);
        }
    }
}