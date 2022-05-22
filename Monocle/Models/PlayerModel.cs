using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Models
{
    internal class PlayerModel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public int Ping { get; set; }
        public Position Position { get; set; }
        public byte Health { get; set; }
        public float Rotation { get; set; }

        public PlayerModel(SteamPlayer player)
        {
            Id = player.playerID.steamID.m_SteamID;
            IsAdmin = player.isAdmin;
            Name = player.player.name;
            Ping = (int)Math.Ceiling(player.ping);
            Position = player.player.transform.position.ToPosition();
            Health = player.player.life.health;
            Rotation = player.player.transform.rotation.eulerAngles.y;
        }

        public PlayerModel(UnturnedPlayer player)
        {
            Id = player.CSteamID.m_SteamID;
            IsAdmin = player.IsAdmin;
            Name = player.DisplayName;
            Ping = (int)Math.Ceiling(player.Ping);
            Position = player.Position.ToPosition();
            Health = player.Health;
            Rotation = player.Rotation;
        }
    }

    internal class PlayerDetailsModel : PlayerModel
    {
        public List<ItemModel> Items { get; set; }

        public PlayerDetailsModel(SteamPlayer player, List<ItemModel> items) : base(player)
        {
            Items = items;
        }
    }
}
