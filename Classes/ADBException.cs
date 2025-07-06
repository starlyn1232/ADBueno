using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ADBueno.Classes
{
    [Serializable]
    public class ADBException : Exception
    {
        public ADBException() { }
        public ADBException(string message) : base(message) { }
        public ADBException(string message, Exception inner) : base(message, inner) { }
        protected ADBException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
