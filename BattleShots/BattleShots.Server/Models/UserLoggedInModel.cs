using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BattleShots.Server.Models
{
    public class UserLoggedInModel
    {
        public string Username { get; set; }

        public string SessionKey { get; set; }
    }
}