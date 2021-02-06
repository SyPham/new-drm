using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMR_API.Helpers
{
    public class ResponseDetail<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; }
    }
}
