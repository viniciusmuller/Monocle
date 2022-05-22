using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Models
{
    internal class VehicleModel
    {
        public bool IsLocked { get; set; }
        public string Name { get; set; }
        public Position Position { get; set; }
        public ushort Id { get; set; }

        public VehicleModel(InteractableVehicle vehicle, string vehicleName)
        {
            IsLocked = vehicle.isLocked;
            Id = vehicle.id;
            Name = vehicleName;
            Position = vehicle.transform.position.ToPosition();
        }
    }
}
