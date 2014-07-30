using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BattleShots.Server.Models
{
    [DataContract]
    public class FiguresPlacingModel
    {
        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "units")]
        public ICollection<UnitBindingModel> Units { get; set; }
    }
}