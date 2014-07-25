using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BattleShots.Server.Models
{
    public class UserLoggingInModel : UserLoggedInModel
    {
        [Required]
        public string Password { get; set; }
    }
}