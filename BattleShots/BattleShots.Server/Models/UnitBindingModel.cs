using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BattleShots.Server.Models
{
    [DataContract]
    public class UnitBindingModel
    {
        [DataMember(Name = "type")]
        [Required]
        public string UnitType { get; set; }

        [DataMember(Name = "row")]
        [Required]
        public int Row { get; set; }

        [DataMember(Name = "col")]
        [Required]
        public int Col { get; set; }

        [DataMember(Name = "rotation")]
        [Required]
        public string Rotation { get; set; }
    }
}