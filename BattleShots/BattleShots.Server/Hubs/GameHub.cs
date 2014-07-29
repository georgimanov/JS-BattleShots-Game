using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using BattleShots.Models;
using BattleShots.Server.Utilities;

namespace BattleShots.Server.Hubs
{
    public class GameHub : Hub
    {
        public void StoreConnectionId(bool firstPlayer, int gameId)
        {
            var context = new ApplicationDbContext();
            string connectionId = Context.ConnectionId;
            var game = context.Games.FirstOrDefault(g => g.Id == gameId);
            if (game == null)
            {
                throw new ServerErrorException("There is no such game.", ErrorType.InvalidGame);
            }

            if (firstPlayer)
            {
                game.FirstPlayerConnId = connectionId;
            }
            else
            {
                game.SecondPlayerConnId = connectionId;
            }

            context.SaveChanges();
        }

        public void InformPlayerPlacedFigures(int gameId, bool firstPlayerPlaced)
        {
            var context = new ApplicationDbContext();
            var game = context.Games.FirstOrDefault(g => g.Id == gameId);
            if (firstPlayerPlaced)
            {
                Clients.Client(game.SecondPlayerConnId).requestBoards();
            }
            else
            {
                Clients.Client(game.FirstPlayerConnId).requestBoards();
            }
        }
    }
}