using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Models
{
    internal class ItemModel
    {
        public string Name;
        public int Amount;
        public ushort Id;
        public byte Durability;

        public ItemModel(ItemJar item, string friendlyName)
        {
            Amount = item.item.amount;
            Name = friendlyName;
            Durability = item.item.durability;
            Id = item.item.id;
        }

        public ItemModel(ItemAsset item, byte durability)
        {
            Amount = item.amount;
            Name = item.FriendlyName;
            Durability = durability;
            Id = item.id;
        }
    }
}
