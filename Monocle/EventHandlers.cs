﻿using Monocle.Api;
using Monocle.Models;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public static PlayerMessageEvent PlayerMessage(UnturnedPlayer player, Color color, EChatMode chatMode, string message)
        {
            var playerModel = new PlayerModel(player);
            return new PlayerMessageEvent(playerModel, color, chatMode, message);
        }

        internal static PlayerJoinOrLeaveEvent PlayerJoinedOrLeft(UnturnedPlayer player)
        {
            var playerModel = new PlayerModel(player);
            return new PlayerJoinOrLeaveEvent(playerModel);
        }
    }
}
