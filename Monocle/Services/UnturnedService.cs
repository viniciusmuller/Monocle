using Monocle.Api;
using Monocle.Exceptions;
using Monocle.Models;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Services
{
    internal class UnturnedService
    {
        private readonly static string MonocleVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public List<PlayerModel> GetPlayers()
        {

            var players = Provider.clients;
            var playerModels = players.ConvertAll(p => new PlayerModel(p));
            return playerModels;
        }
        
        public PlayerDetailsModel GetPlayerDetails(ulong? userId)
        {
            if (userId == null)
            {
                throw new ApiException(ErrorType.InvalidRequestData, $"userId was not provided");
            }

            var result = Utils.TryGetPlayer(userId.Value, out var client);
            if (!result)
            {
                throw new ApiException(ErrorType.UserNotFound, $"The user of ID {userId} was not found in the server.");
            }

            var playerInventory = FetchInventoryItems(client.player.inventory.items);
            return new PlayerDetailsModel(client, playerInventory);
        }

        public ServerInfoModel GetServerInfo()
        {
            return new ServerInfoModel
            {
                MapName = Provider.map,
                MaxPlayers = Provider.maxPlayers,
                ServerName = Provider.serverName,
                QueueSize = Provider.queueSize,
                CurrentPlayers = Provider.clients.Count,
                UnturnedVersion = Provider.APP_VERSION,
                MonocleVersion = MonocleVersion,
                PlayersInQueue = Provider.queuePosition,
                WorldSize = ParseLevelSize(Level.info.size)
            };
        }

        public List<BarricadeModel> GetBarricades()
        {
            var barricades = BarricadeManager.regions.Cast<BarricadeRegion>()
                                                     .SelectMany(x => x.drops);

            var barricadeModels = barricades.Select(b => new BarricadeModel(b));
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

        public List<ItemModel> FetchInventoryItems(Items[] inventory)
        {
            var playerInventory = new List<ItemModel>();
            foreach (var itemPack in inventory)
            {
                if (itemPack == null)
                {
                    continue;
                }

                foreach (var item in itemPack.items)
                {
                    playerInventory.Add(new ItemModel(item, item.GetName()));
                }
            }
            return playerInventory;
        }

        private int ParseLevelSize(ELevelSize levelSize)
        {
            return levelSize switch
            {
                ELevelSize.TINY => 512,
                ELevelSize.SMALL => 1024,
                ELevelSize.MEDIUM => 2048,
                ELevelSize.LARGE => 4096,
                ELevelSize.INSANE => 8192,
                _ => 0,
            };
        }
    }
}
