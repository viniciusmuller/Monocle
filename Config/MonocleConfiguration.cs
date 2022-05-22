using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Config
{
    public class MonocleConfiguration : IRocketPluginConfiguration
    {
        public string BindAddress { get; set; }
        public int ListenPort { get; set; }

        public void LoadDefaults()
        {
            ListenPort = 55554;
            BindAddress = "127.0.0.1";
        }
    }
}
