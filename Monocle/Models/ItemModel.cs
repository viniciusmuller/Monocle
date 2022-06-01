using Monocle.Api;
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
        public int Amount { get; set; }
        public ushort Id { get; set; }
        public byte Durability { get; set; }
        public ItemType Type { get; set; }
        public ItemRarity Rarity { get; set; }

        public ItemModel(ItemJar item, ItemAsset asset)
        {
            Amount = item.item.amount;
            Name = asset.FriendlyName;
            Durability = item.item.durability;
            Id = item.item.id;
            Type = (ItemType)asset.type;
            Rarity = (ItemRarity)asset.rarity;
        }

        public ItemModel(ItemAsset item, byte? durability)
        {
            Amount = item.amount;
            Name = item.FriendlyName;
            Durability = durability ?? 100;
            Id = item.id;
            Type = (ItemType)item.type;
            Rarity = (ItemRarity)item.rarity;
        }
    }
}
