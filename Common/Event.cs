using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public class Event
    {
        public enum EventType { request, response }

        public EventType Type { get; set; }
        public dynamic Content { get; set; }
    }
}
