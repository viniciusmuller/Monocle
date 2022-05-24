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
        const ushort NotFoundId = 0;

        // TODO: Get group info
        public ulong Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public int Ping { get; set; }
        public Position Position { get; set; }
        public byte Health { get; set; }
        public float Rotation { get; set; }
        public ItemModel? Hat { get; set; }
        public ItemModel? Vest { get; set; }
        public ItemModel? Backpack { get; set; }
        public ItemModel? Pants { get; set; }
        public ItemModel? Shirt { get; set; }
        public ItemModel? Mask { get; set; }
        public ItemModel? Glasses { get; set; }
        public ItemModel? Primary { get; set; }
        public ItemModel? Secondary { get; set; }

        public PlayerModel(SteamPlayer player)
        {
            Id = player.playerID.steamID.m_SteamID;
            IsAdmin = player.isAdmin;
            Name = player.player.name;
            Ping = (int)Math.Ceiling(player.ping);
            Position = player.player.transform.position.ToPosition();
            Health = player.player.life.health;
            Rotation = player.player.transform.rotation.eulerAngles.y;

            var clothing = player.player.clothing;

            if (clothing.hat != NotFoundId)
            {
                Hat = new ItemModel(Utils.FindItem(clothing.hat)!, clothing.hatQuality);
            }

            if (clothing.vest != NotFoundId)
            {
                Vest = new ItemModel(Utils.FindItem(clothing.vest)!, clothing.vestQuality);
            }

            if (clothing.backpack != NotFoundId)
            {
                Backpack = new ItemModel(Utils.FindItem(clothing.backpack)!, clothing.backpackQuality);
            }

            if (clothing.pants != NotFoundId)
            {
                Pants = new ItemModel(Utils.FindItem(clothing.pants)!, clothing.pantsQuality);
            }

            if (clothing.shirt != NotFoundId)
            {
                Shirt = new ItemModel(Utils.FindItem(clothing.pants)!, clothing.pantsQuality);
            }

            if (clothing.mask != NotFoundId)
            {
                Mask = new ItemModel(Utils.FindItem(clothing.mask)!, clothing.maskQuality);
            }

            if (clothing.glasses != NotFoundId)
            {
                Glasses = new ItemModel(Utils.FindItem(clothing.glasses)!, clothing.glassesQuality);
            }

            var primary = player.player.inventory.items[0].items.FirstOrDefault();
            if (primary != null)
            {
                Primary = new ItemModel(primary, primary.GetName());
            }

            var secondary = player.player.inventory.items[1].items.FirstOrDefault();
            if (secondary != null)
            {
                Secondary = new ItemModel(secondary, secondary.GetName());
            }
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
