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

        public ItemModel(ItemJar item, string name)
        {
            Amount = item.item.amount;
            Name = name;
            Durability = item.item.durability;
            Id = item.item.id;
        }
    }
}
