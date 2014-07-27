using BattleShots.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BattleShots.Server.Controllers
{
    public class BaseApiController : ApiController
    {
        private const int Sha1Length = 40;

        private static readonly Dictionary<string, HttpStatusCode> ErrorsToStatusCodes = new Dictionary<string, HttpStatusCode>();

        static BaseApiController()
        {
            ErrorsToStatusCodes[ErrorType.InvalidUser] = HttpStatusCode.Unauthorized;
            ErrorsToStatusCodes[ErrorType.InvalidUserAuthentication] = HttpStatusCode.Unauthorized;
            ErrorsToStatusCodes[ErrorType.InvalidPassword] = HttpStatusCode.Unauthorized;

            ErrorsToStatusCodes[ErrorType.DuplicateUser] = HttpStatusCode.Conflict;

            ErrorsToStatusCodes[ErrorType.InvalidUsernameLength] = HttpStatusCode.BadRequest;
            ErrorsToStatusCodes[ErrorType.InvalidUsernameCharacters] = HttpStatusCode.BadRequest;
            ErrorsToStatusCodes[ErrorType.InvalidGame] = HttpStatusCode.BadRequest;
            ErrorsToStatusCodes[ErrorType.InvalidInput] = HttpStatusCode.BadRequest;

            ErrorsToStatusCodes[ErrorType.GeneralError] = HttpStatusCode.InternalServerError;
        }

        public BaseApiController()
        {
        }

        protected IHttpActionResult PerformOperation<T>(Func<T> action)
        {
            try
            {
                var result = action();
                return Ok(result);
            }
            catch (ServerErrorException ex)
            {
                return BuildErrorResponse(ex.Message, ex.ErrorType);
            }
            catch (Exception ex)
            {
                var errType = ErrorType.GeneralError;
                return BuildErrorResponse(ex.Message, errType);
            }
        }

        protected IHttpActionResult PerformOperationWithNoContent<T>(Func<T> action)
        {
            try
            {
                var result = action();
                return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NoContent));
            }
            catch (ServerErrorException ex)
            {
                return BuildErrorResponse(ex.Message, ex.ErrorType);
            }
            catch (Exception ex)
            {
                var errType = ErrorType.GeneralError;
                return BuildErrorResponse(ex.Message, errType);
            }
        }

        protected string GetSessionKey()
        {
            IEnumerable<string> sessionKeys;
            if (!Request.Headers.TryGetValues("X-SessionKey", out sessionKeys))
            {
                throw new ServerErrorException("There is no session key provided", ErrorType.InvalidUserAuthentication);
            }

            else
            {
                return sessionKeys.First();
            }
        }

        private IHttpActionResult BuildErrorResponse(string message, string errType)
        {
            var httpError = new HttpError(message);
            httpError["errType"] = errType;
            var statusCode = ErrorsToStatusCodes[errType];
            return ResponseMessage(Request.CreateErrorResponse(statusCode, httpError));
        }

        protected void ValidatePassword(string password)
        {
            if (password.Length != Sha1Length)
            {
                throw new ServerErrorException("The password is invalid.", ErrorType.InvalidPassword);
            }
        }
    }
}
