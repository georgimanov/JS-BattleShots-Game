using System;
using System.Linq;
using System.Runtime.Serialization;

namespace BattleShots.Server.Models
{
    [DataContract]
    public class AttackingModel
    {
        [DataMember(Name = "row")]
        public int Row { get; set; }

        [DataMember(Name = "col")]
        public int Col { get; set; }
    }
}