using BattleShots.Models;
using BattleShots.Server.Models;
using BattleShots.Server.Utilities;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BattleShots.Server.Controllers
{
    public class GamesController : BaseApiController
    {
        private const string OpenState = "open";
        private const string InProgressState = "in-progress";

        // POST api/games/new
        [HttpPost]
        public IHttpActionResult New([FromBody]GameBindingModel gameModel)
        {
            var response = this.PerformOperation(() =>
            {
                string sessionKey = GetSessionKey();
                if (!string.IsNullOrEmpty(gameModel.Password))
                {
                    ValidatePassword(gameModel.Password);
                }

                var context = new ApplicationDbContext();
                using (context)
                {
                    var state = context.GameStates.First(s => s.State == OpenState);
                    var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                    if (user == null)
                    {
                        throw new ServerErrorException("The user does not exist.", ErrorType.InvalidUser);
                    }

                    var game = new Game()
                    {
                        Title = gameModel.Title,
                        Password = gameModel.Password,
                        FirstPlayer = user,
                        CurrentTurn = 1,
                        State = state
                    };

                    context.Games.Add(game);
                    context.SaveChanges();

                    return new[] { game }.Select(GameViewModel.FromGame).First();
                }
            });

            return response;
        }

        // POST api/games/join/5
        [HttpPost, ActionName("join")]
        public IHttpActionResult Join(int id, [FromBody]GameBindingModel gameModel)
        {
            var response = this.PerformOperation(() =>
            {
                string sessionKey = GetSessionKey();
                if (!string.IsNullOrEmpty(gameModel.Password))
                {
                    ValidatePassword(gameModel.Password);
                }

                var context = new ApplicationDbContext();
                using (context)
                {
                    var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
                    if (user == null)
                    {
                        throw new ServerErrorException("The user does not exist.", ErrorType.InvalidUser);
                    }

                    var game = context.Games.FirstOrDefault(g => g.Id == id);
                    var stateOpen = context.GameStates.First(s => s.State == OpenState);
                    if (game == null)
                    {
                        throw new ServerErrorException("The game does not exist.", ErrorType.InvalidGame);
                    }
                    
                    if (!string.IsNullOrEmpty(game.Password) && game.Password != gameModel.Password)
                    {
                        throw new ServerErrorException("The entered password for this game is wrong.", ErrorType.InvalidPassword);
                    }
                    
                    if (game.State != stateOpen)
                    {
                        throw new ServerErrorException("The game is not open.", ErrorType.InvalidGame);
                    }

                    if (game.FirstPlayer == user)
                    {
                        throw new ServerErrorException("You cannot play with yourself.", ErrorType.GeneralError);
                    }

                    game.SecondPlayer = user;
                    var stateInProgress = context.GameStates.First(s => s.State == InProgressState);
                    game.State = stateInProgress;
                    context.SaveChanges();

                    return new[] { game }.Select(GameViewModel.FromGame).First();
                }
            });

            return response;
        }

        // GET api/games/open
        [HttpGet]
        public IHttpActionResult Open()
        {
            var response = this.PerformOperation(() =>
            {
                var context = new ApplicationDbContext();

                return context.Games
                    .Where(g => g.State.State == OpenState)
                    .ToList()
                    .Select(GameViewModel.FromGame);
            });

            return response;
        }

        // GET api/games/in-progress
        [HttpGet]
        [ActionName("in-progress")]
        public IHttpActionResult InProgress()
        {
            var response = this.PerformOperation(() =>
            {
                var context = new ApplicationDbContext();

                return context.Games
                    .Where(g => g.State.State == InProgressState)
                    .ToList()
                    .Select(GameViewModel.FromGame);
            });

            return response;
        }
    }
}
