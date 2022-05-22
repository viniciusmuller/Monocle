using Monocle.Api;
using Monocle.Models;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle
{
    internal static class EventHandlers
    {
        public static PlayerDeathEvent PlayerDeath(UnturnedPlayer deadPlayer, CSteamID murdererId, EDeathCause cause)
        {
            var dead = new PlayerModel(deadPlayer);
            var result = Utils.TryGetPlayer(murdererId, out var killer);
            var killerModel = result ? new PlayerModel(killer) : null;
            return new PlayerDeathEvent(dead, killerModel, cause.ToString());
        }

        public static PlayerMessageEvent PlayerMessage(UnturnedPlayer player, EChatMode chatMode, string message)
        {
            var playerModel = new PlayerModel(player);
            return new PlayerMessageEvent(playerModel, chatMode, message);
        }
    }
}
