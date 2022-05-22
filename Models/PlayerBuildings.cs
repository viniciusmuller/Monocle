using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Models
{
    abstract class PlayerBuilding
    {
        public string Name { get; set; }
        public float Health { get; set; }
        public Position Position { get; set; }
    }

    class StructureModel : PlayerBuilding
    {
        public StructureModel(StructureDrop drop)
        {
            Name = drop.asset.name;
            Position = drop.model.position.ToPosition();
            Health = drop.asset.health;
        }
    }

    class BarricadeModel : PlayerBuilding
    {
        public BarricadeModel(BarricadeDrop drop)
        {
            Name = drop.asset.name;
            Position = drop.model.position.ToPosition();
            Health = drop.asset.health;
        }
    }

}
