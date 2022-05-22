using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle
{
    internal static class Utils
    {
        public static bool TryGetPlayer(CSteamID playerId, out SteamPlayer player)
        {
            @player = Provider.clients.Where(p => p.playerID.steamID == playerId).FirstOrDefault();
            return @player != null;
        }

        public static bool TryGetPlayer(ulong playerId, out SteamPlayer player)
        {
            var id = new CSteamID(playerId);
            return TryGetPlayer(id, out player);
        }
    }
}
