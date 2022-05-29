using Monocle.Api;
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
        public string OwnerId { get; set; }
        public ushort Battery { get; set; }
        public ushort Fuel { get; set; }
        public ushort Health { get; set; }
        public VehicleType Type { get; set; }

        public VehicleModel(InteractableVehicle vehicle, string vehicleName)
        {
            IsLocked = vehicle.isLocked;
            Id = vehicle.id;
            Name = vehicleName;
            Position = vehicle.transform.position.ToPosition();
            Battery = vehicle.batteryCharge;
            Health = vehicle.health;
            Fuel = vehicle.fuel;
            OwnerId = vehicle.lockedOwner.ToString();
            Type = GetVehicleType(vehicle.asset.engine);
        }

        private VehicleType GetVehicleType(EEngine engine)
        {
            return engine switch
            {
                EEngine.CAR => VehicleType.Car,
                EEngine.TRAIN => VehicleType.Train,
                EEngine.BLIMP => VehicleType.Blimp,
                EEngine.HELICOPTER => VehicleType.Helicopter,
                EEngine.PLANE => VehicleType.Plane,
                EEngine.BOAT => VehicleType.Boat,
                _ => throw new NotImplementedException()
            };
        }
    }
}
