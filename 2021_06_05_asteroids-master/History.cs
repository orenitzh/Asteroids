using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Asteroids
{
    public class History
    {
        public int HistoryID {get;set;}
        public int PlayerID { get; set; }
        public int Score { get; set; }
        public DateTime DateTime { get; set; }
    }
}
