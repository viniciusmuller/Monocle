using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Api
{
    internal enum ErrorType
    {
        InvalidRequestType,
        UserNotFound,
        InvalidRequestData,
    }

    internal enum MessageKind
    {
        Response,
        Event,
        Error
    }

    internal enum RequestType
    {
        Authenticate,
        Players,
        PlayerDetails,
        Structures,
        Barricades,
        Vehicles,
        ServerInfo,
        PlayerScreenshot,
    }

    internal enum ResponseType
    {
        Players,
        CurrentBuildings,
        SuccessfulLogin,
        Vehicles,
        Barricades,
        Structures,
        ServerInfo,
        PlayerScreenshot,
    }

    internal enum EventType
    {
        PlayerDeath,
        PlayerMessage,
        PlayerJoined,
        PlayerLeft,
    }

    public enum AuthorizedUserType
    {
        Observer,
        Administrator
    }

    enum ChatMode
    {
        Global = 1,
        Local,
        Group,
        Say,
        Welcome,
    }

    enum VehicleType
    {
        Car = 1,
        Plane,
        Helicopter,
        Blimp,
        Boat,
        Train,
    }

    enum ItemType
    {
        Hat = 1,
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
