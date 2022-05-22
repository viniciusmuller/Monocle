using Monocle.Api;
using Monocle.Exceptions;
using Monocle.Models;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Services
{
    internal class UnturnedService
    {
        public List<PlayerModel> GetPlayers()
        {

            var players = Provider.clients;
            var playerModels = players.ConvertAll(p => new PlayerModel(p));
            return playerModels;
        }
        
        public PlayerModel GetPlayerInfo(string? userId)
        {
            var client = Provider.clients.Where(p => p.playerID.steamID.ToString() == userId).FirstOrDefault();
            if (client == null)
            {
                throw new ApiException(ErrorType.UserNotFound, $"The user of ID {userId} was not found in the server.");
            }

            var playerInventory = FetchInventoryItems(client);
            return new PlayerModel(client, playerInventory);
        }

        public List<BarricadeModel> GetBarricades()
        {
            var barricades = BarricadeManager.regions.Cast<BarricadeRegion>()
                                                     .SelectMany(x => x.drops);

            var barricadeModels = barricades.Select(s => new BarricadeModel(s));
            return barricadeModels.ToList();
        }

        public List<StructureModel> GetStructures()
        {
            var structures = StructureManager.regions.Cast<StructureRegion>()
                                                     .SelectMany(x => x.drops);

            var structureModels = structures.Select(s => new StructureModel(s));
            return structureModels.ToList();

        }

        public List<VehicleModel> GetVehicles()
        {
            var vehicles = VehicleManager.vehicles;
            var vehicleModels = vehicles.Select(v =>
            {
                var name = Assets.find(EAssetType.VEHICLE, v.id).FriendlyName;
                return new VehicleModel(v, name);
            });
            return vehicleModels.ToList();
        }

        List<ItemModel> FetchInventoryItems(SteamPlayer player)
        {
            var playerInventory = new List<ItemModel>();
            foreach (var itemPack in player.player.inventory.items)
            {
                if (itemPack == null)
                {
                    continue;
                }

                foreach (var item in itemPack.items)
                {
                    var itemName = Assets.find(EAssetType.ITEM, item.item.id).FriendlyName;
                    playerInventory.Add(new ItemModel(item, itemName));
                }
            }
            return playerInventory;
        }

    }
}
