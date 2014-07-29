using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleShots.Server.Utilities
{
    internal class ErrorType
    {
        internal const string InvalidUser = "ERR_INV_USR";
        internal const string InvalidUserAuthentication = "INV_USR_AUTH";
        internal const string InvalidPassword = "ERR_INV_PASS";

        internal const string DuplicateUser = "ERR_DUP_USR";

        internal const string InvalidUsernameLength = "INV_USR_LEN";
        internal const string InvalidUsernameCharacters = "INV_USR_CHARS";
        internal const string InvalidGame = "ERR_INV_GAME";
        internal const string InvalidInput = "ERR_INV_INPUT";

        internal const string GeneralError = "ERR_GEN_SVR";
    }
}