using Monocle.Config;
using Monocle.Services;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;

namespace Monocle
{
    public class Monocle : RocketPlugin<MonocleConfiguration>
    {
        private ServerService? serverService { get; set; }

        protected override void Load()
        {
            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!");
            var bindAddress = Configuration.Instance.BindAddress;
            var listenPort = Configuration.Instance.ListenPort;
            var unturnedService = new UnturnedService();
            serverService = new ServerService(Configuration.Instance, unturnedService);
            serverService.Start(bindAddress, listenPort);
        }

        protected override void Unload()
        {
            serverService!.Stop();
            Logger.Log($"{Name} {Assembly.GetName().Version} has been unloaded!");
        }
    }
}