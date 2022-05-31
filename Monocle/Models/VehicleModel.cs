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
        public string InstanceId { get; set; }
        public string OwnerId { get; set; }
        public ushort Battery { get; set; }
        public ushort Fuel { get; set; }
        public ushort Health { get; set; }
        public VehicleType Type { get; set; }

        public VehicleModel(InteractableVehicle vehicle, string vehicleName)
        {
            IsLocked = vehicle.isLocked;
            Id = vehicle.id;
            InstanceId = vehicle.instanceID.ToString();
            Name = vehicleName;
            Position = vehicle.transform.position.ToPosition();
            Battery = vehicle.batteryCharge;
            Health = vehicle.health;
            Fuel = vehicle.fuel;
            OwnerId = vehicle.lockedOwner.ToString();
            Type = (VehicleType)vehicle.asset.engine;
        }
    }
}
