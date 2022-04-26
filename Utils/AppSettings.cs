using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Utils
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string SqlLoginUser { get; set; }
        public string SqlUserPassword { get; set; }
        public string SqlServerUrl { get; set; }
        public int CommandTimeout { get; set; }
    }
}
