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
            var unturnedService = new UnturnedService();
            ServerService = new ServerService(Configuration.Instance, unturnedService);
            ServerService.Start();
        }

        protected override void Unload()
        {
            ServerService.Stop();
            Logger.Log($"{Name} {Assembly.GetName().Version} has been unloaded!");
        }
    }
}