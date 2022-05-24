using Monocle.Config;
using Monocle.Services;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;

namespace Monocle
{
    public class Monocle : RocketPlugin<MonocleConfiguration>
    {
        private ServerService ServerService { get; set; }

        protected override void Load()
        {
            // TODO: Use rocket dependency injection
            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!");
            var bindAddress = Configuration.Instance.BindAddress;
            var listenPort = Configuration.Instance.ListenPort;
            var unturnedService = new UnturnedService();
            ServerService = new ServerService(Configuration.Instance, unturnedService);
            // TODO: No need to pass these since server service already has the Configuration instance
            ServerService.Start(bindAddress, listenPort);
        }

        protected override void Unload()
        {
            ServerService.Stop();
            Logger.Log($"{Name} {Assembly.GetName().Version} has been unloaded!");
        }
    }
}