using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Models
{
    internal class ServerInfoModel
    {
        public string ServerName { get; set; }
        public string MonocleVersion { get; set; }
        public string UnturnedVersion { get; set; }
        public string MapName { get; set; }
        public int MaxPlayers { get; set; }
        public int CurrentPlayers { get; set; }
        public int WorldSize { get; set; }
        public int BorderSize { get; set; }
        public int QueueSize { get; set; }
        public int PlayersInQueue { get; set; }
        public TimeModel GameTime { get; set; }
    }

    public class TimeModel
    {
        public uint Minutes;
        public uint Hours;
    }
}
