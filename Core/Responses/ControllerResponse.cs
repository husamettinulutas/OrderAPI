using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Responses
{
    public class ControllerResponse
    {
        public int ResponseCode { get; set; }

        public int TotalCount { get; set; }

        public int ResultCount { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public int PageCount { get; set; }

        public dynamic Result { get; set; }

        public string SuccessMessage { get; set; }

        public string WarningMessage { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }
}
