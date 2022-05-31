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
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public int Ping { get; set; }
        public Position Position { get; set; }
        public byte Health { get; set; }
        public float Rotation { get; set; }
        public EquipmentModel Equipment { get; set; }
        public int Reputation { get; set; }
        public DateTimeOffset Joined { get; set; }
        public string GroupId { get; set; }
        public List<ItemModel> Items { get; set; }

        public PlayerModel(SteamPlayer player)
        {
            Id = player.playerID.steamID.ToString();
            IsAdmin = player.isAdmin;
            Name = player.player.name;
            Ping = (int)Math.Ceiling(player.ping * 1000);
            Position = player.player.transform.position.ToPosition();
            Health = player.player.life.health;
            Rotation = player.player.transform.rotation.eulerAngles.y;
            Equipment = new EquipmentModel(player);
            Joined = DateTime.UtcNow; // TODO: Correctly parse date
            Reputation = player.player.skills.reputation;
            GroupId = player.player.quests.groupID.ToString();
            Items = GetItemsFromInventory(player.player.inventory);
        }

        public PlayerModel(UnturnedPlayer player)
        {
            Id = player.CSteamID.ToString();
            IsAdmin = player.IsAdmin;
            Name = player.DisplayName;
            Ping = (int)Math.Ceiling(player.Ping * 1000);
            Position = player.Position.ToPosition();
            Health = player.Health;
            Rotation = player.Rotation;
            Equipment = new EquipmentModel(player);
            Joined = DateTime.UtcNow; // TODO: Correctly parse date
            GroupId = player.SteamGroupID.ToString();
            Reputation = player.Reputation;
            Items = GetItemsFromInventory(player.Inventory);
        }
        
        private List<ItemModel> GetItemsFromInventory(PlayerInventory inventory)
        {
            var items = new List<ItemModel>();
            var pages = inventory.items.Take(inventory.items.Count() - 2); // Ignore storage and ground pages

            foreach (var page in pages)
            {
                var pageItems = page.items.Select(i => new ItemModel(i, Utils.FindItem(i.item.id)!));
                items.AddRange(pageItems);
            }

            return items;
        }
    }
}
