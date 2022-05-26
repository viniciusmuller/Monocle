using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Models
{
    internal class ServerInfoModel
    {
        public int MaxPlayers { get; set; }
        public int CurrentPlayers { get; set; }
        public string MapName { get; set; }
        public int WorldSize;
    }
}
