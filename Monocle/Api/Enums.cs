using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Api
{
    public enum AuthorizedUserType
    {
        Observer,
        Administrator
    }

    enum ChatMode
    {
        Global,
        Local,
        Group,
        Say,
        Welcome,
    }

    enum VehicleType
    {
        Car,
        Plane,
        Helicopter,
        Blimp,
        Boat,
        Train,
    }

    enum ItemType
    {
        Hat,
        Pants,
        Shirt,
        Mask,
        Backpack,
        Vest,
        Glasses,
        Gun,
        Sight,
        Tactical,
        Grip,
        Barrel,
        Magazine,
        Food,
        Water,
        Medical,
        Melee,
        Fuel,
        Tool,
        Barricade,
        Storage,
        Beacon,
        Farm,
        Trap,
        Structure,
        Supply,
        Throwable,
        Grower,
        Optic,
        Refill,
        Fisher,
        Cloud,
        Map,
        Key,
        Box,
        ArrestStart,
        ArrestEnd,
        Tank,
        Generator,
        Detonator,
        Charge,
        Library,
        Filter,
        Sentry,
        VehicleRepairTool,
        Tire,
        Compass,
        OilPump
    }

    enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythical
    }
}
