using BattleShots.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using BattleShots.Server.Models;
using BattleShots.Server.Utilities;
using System.Web.Http.ValueProviders;

namespace BattleShots.Server.Controllers
{
    public class AccountController : BaseApiController
    {
        private const int MinUsernameLength = 6;
        private const int MaxUsernameLength = 40;
        private const int Sha1Length = 40;
        private const int SessionKeyLength = 50;
        private const string ValidUsernameCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_.";
        private const string SessionKeyCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        private static readonly Random random = new Random();

        // POST api/account/register
        [HttpPost]
        [ActionName("register")]
        public IHttpActionResult RegisterUser([FromBody]UserRegisteringModel userModel)
        {
            var response = this.PerformOperation(() =>
                {
                    var context = new ApplicationDbContext();
                    using (context)
                    {
                        this.ValidateUsername(userModel.Username);
                        this.ValidatePassword(userModel.Password);

                        User user = context.Users
                            .FirstOrDefault(u => u.Username == userModel.Username);
                        if (user != null)
                        {
                            throw new ServerErrorException("A user with this username already exists.", ErrorType.DuplicateUser);
                        }

                        user = new User()
                        {
                            Username = userModel.Username,
                            Password = userModel.Password,
                            Score = 0

                        };
                        context.Users.Add(user);
                        context.SaveChanges();

                        user.SessionKey = this.GenerateSessionKey(user.Id);
                        context.SaveChanges();

                        UserLoggedInModel loggedInUser = new UserLoggedInModel()
                        {
                            Username = user.Username,
                            SessionKey = user.SessionKey
                        };

                        return loggedInUser;
                    }
                });

            return response;
        }

        // POST api/account/login
        [HttpPost]
        [ActionName("login")]
        public IHttpActionResult LoginUser(UserLoggingInModel userModel)
        {
            var response = this.PerformOperation(() =>
                {
                    var context = new ApplicationDbContext();
                    using (context)
                    {
                        this.ValidateUsername(userModel.Username);
                        this.ValidatePassword(userModel.Password);

                        User existingUser = context.Users
                            .FirstOrDefault(u => u.Username == userModel.Username);
                        if (existingUser == null)
                        {
                            throw new ServerErrorException("The user does not exist.", ErrorType.InvalidUser);
                        }

                        if (existingUser.SessionKey == null)
                        {
                            existingUser.SessionKey = this.GenerateSessionKey(existingUser.Id);
                            context.SaveChanges();
                        }

                        UserLoggedInModel loggedUser = new UserLoggedInModel()
                        {
                            Username = existingUser.Username,
                            SessionKey = existingUser.SessionKey
                        };

                        return Created(loggedUser.Username, loggedUser);
                    }
                });

            return response;
        }

        // PUT api/account/logout
        [HttpPut]
        [ActionName("logout")]
        public IHttpActionResult LogoutUser(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var response = this.PerformOperation(() =>
                {
                    var context = new ApplicationDbContext();
                    using (context)
                    {
                        User existingUser = context.Users
                                                   .FirstOrDefault(u => u.SessionKey == sessionKey);
                        if (existingUser == null)
                        {
                            throw new ServerErrorException("The user does not exist or is already logged out.", ErrorType.InvalidUser);
                        }

                        existingUser.SessionKey = null;
                        context.SaveChanges();

                        return Ok();
                    }
                });

            return response;
        }

        internal static User GetUserBySessionKey(ApplicationDbContext context, string sessionKey)
        {
            var user = context.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
            if (user == null)
            {
                throw new ServerErrorException("The user does not exist or is already logged out.", ErrorType.InvalidUser);
            }

            return user;
        }

        private void ValidateUsername(string username)
        {
            if (username == null || username.Length < MinUsernameLength || username.Length > MaxUsernameLength)
            {
                throw new ServerErrorException(
                    string.Format("The username should be between {0} and {1} symbols long.", MinUsernameLength, MaxUsernameLength),
                    ErrorType.InvalidUsernameLength);
            }
            else if (username.Any(ch => !ValidUsernameCharacters.Contains(ch)))
            {
                throw new ServerErrorException("The username contains invalid characters.", ErrorType.InvalidUsernameCharacters);
            }
        }


        private void ValidatePassword(string password)
        {
            if (password.Length != Sha1Length)
            {
                throw new ServerErrorException("The password is invalid.", ErrorType.InvalidUserAuthentication);
            }
        }

        private string GenerateSessionKey(int userId)
        {
            StringBuilder sessionKey = new StringBuilder();
            sessionKey.Append(userId);
            while (sessionKey.Length < SessionKeyLength)
            {
                int index = random.Next(SessionKeyCharacters.Length);
                sessionKey.Append(SessionKeyCharacters[index]);
            }

            return sessionKey.ToString();
        }
    }
}