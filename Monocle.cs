using Monocle.Config;
using Monocle.Services;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;

namespace Monocle
{
    public class Monocle : RocketPlugin<MonocleConfiguration>
    {
        private Server server { get; set; }

        protected override void Load()
        {
            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!");
            server = new Server();
            var bindAddress = Configuration.Instance.BindAddress;
            var listenPort = Configuration.Instance.ListenPort;
            server.Start(bindAddress, listenPort);
        }

        protected override void Unload()
        {
            server.Stop();
            Logger.Log($"{Name} {Assembly.GetName().Version} has been unloaded!");
        }
    }
}