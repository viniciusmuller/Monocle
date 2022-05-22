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
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public int Ping { get; set; }
        public List<ItemModel> Items { get; set; }
        public Position? Position { get; set; }
        public byte? Health { get; set; }

        public PlayerModel(SteamPlayer player)
        {
            Id = player.playerID.steamID.ToString();
            IsAdmin = player.isAdmin;
            Name = player.player.name;
            Items = new List<ItemModel>();
            Ping = (int)Math.Ceiling(player.ping);
            Position = player.player.transform.position.ToPosition();
            // Health = client.player.life.health // TODO: Get correct player life
        }

        public PlayerModel(SteamPlayer player, List<ItemModel> items) : this(player)
        {
            Items = items;
        }
    }

}
