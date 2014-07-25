using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BattleShots.Server.Utilities
{
    [DataContract(Name = "Error")]
    public class ServerErrorException : Exception
    {
        public ServerErrorException()
            : base()
        {
        }

        public ServerErrorException(string msg)
            : base(msg)
        {
        }

        public ServerErrorException(string msg, string errType)
            : base(msg)
        {
            this.ErrorType = errType;
        }

        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        public string ErrorType { get; set; }
    }
}