using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Models
{
    internal class EquipmentModel
    {
        const ushort NotFoundId = 0;

        public ItemModel? Hat { get; set; }
        public ItemModel? Vest { get; set; }
        public ItemModel? Backpack { get; set; }
        public ItemModel? Pants { get; set; }
        public ItemModel? Shirt { get; set; }
        public ItemModel? Mask { get; set; }
        public ItemModel? Glasses { get; set; }
        public ItemModel? Primary { get; set; }
        public ItemModel? Secondary { get; set; }

        public EquipmentModel(SteamPlayer player)
        {
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
                Shirt = new ItemModel(Utils.FindItem(clothing.shirt)!, clothing.shirtQuality);
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

        public EquipmentModel(UnturnedPlayer player)
        {
            // TODO: Handle unturned player (or figure out best way to get a SteamPlayer from an UnturnedPlayer)
        }
    }
}
