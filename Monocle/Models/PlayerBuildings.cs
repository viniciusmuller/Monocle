using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Models
{
    class StructureModel
    {
        public string Name { get; set; }
        public float Health { get; set; }
        public Position Position { get; set; }
        public string GroupId { get; set; }
        public string OwnerId { get; set; }

        public StructureModel(StructureDrop drop)
        {
            Name = drop.asset.name;
            Position = drop.model.position.ToPosition();
            Health = drop.asset.health;

            var serverSideData = drop.GetServersideData();
            GroupId = serverSideData.group.ToString();
            OwnerId = serverSideData.owner.ToString();
        }
    }

    class BarricadeModel
    {
        public string Name { get; set; }
        public float Health { get; set; }
        public Position Position { get; set; }
        public List<ItemModel>? Items { get; set; }
        public string GroupId { get; set; }
        public string OwnerId { get; set; }

        public BarricadeModel(BarricadeDrop drop)
        {
            Name = drop.asset.name;
            Position = drop.model.position.ToPosition();
            Health = drop.asset.health;

            var serverSideData = drop.GetServersideData();
            GroupId = serverSideData.group.ToString();
            OwnerId = serverSideData.owner.ToString();

            var storage = drop.interactable as InteractableStorage;
            if (storage != null)
            {
                Items = storage.items.items
                    .Select(i => new ItemModel(i, Utils.FindItem(i.item.id)!))
                    .ToList();
            }
        }
    }
}
