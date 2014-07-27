using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BattleShots.Server.Models
{
    [DataContract]
    public class GameBindingModel
    {
        [DataMember(Name = "title")]
        [Required, StringLength(40, MinimumLength = 6, ErrorMessage = "The game title must be between {2} and {1} characters long.")]
        public string Title { get; set; }

        [DataMember(Name = "password")]
        [Required]
        public string Password { get; set; }
    }
}