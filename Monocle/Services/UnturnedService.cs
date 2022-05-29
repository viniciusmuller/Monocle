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
            var playerModels = Provider.clients.ConvertAll(p => new PlayerModel(p));
            return playerModels;
        }

        public void ScreenshotPlayer(string playerId, Guid socketId, Action<string, Guid, byte[]> callback)
        {
            // TODO: Improve code
            var client = Provider.clients.Find(p => p.playerID.steamID.ToString() == playerId);
            client.player.sendScreenshot(CSteamID.Nil, (player, jpg) => {
                callback(playerId, socketId, jpg);
            });
        }

        public ServerInfoModel GetServerInfo()
        {
            var mapImage = new byte[10]; // TODO: Read image
            
            return new ServerInfoModel
            {
                MapName = Provider.map,
                MaxPlayers = Provider.maxPlayers,
                ServerName = Provider.serverName,
                QueueSize = Provider.queueSize,
                CurrentPlayers = Provider.clients.Count,
                UnturnedVersion = Provider.APP_VERSION,
                MonocleVersion = MonocleVersion,
                MapImageEncoded = Convert.ToBase64String(mapImage),
                PlayersInQueue = Provider.queuePosition,
                WorldSize = Level.size,
                BorderSize = Level.border,
            };
        }

        public List<BarricadeModel> GetBarricades()
        {
            if (BarricadeManager.regions == null)
            {
                return new List<BarricadeModel>();
            }

            var barricades = BarricadeManager.regions.Cast<BarricadeRegion>()
                                                     .SelectMany(x => x.drops);

            var barricadeModels = barricades.Select(b => new BarricadeModel(b));
            return barricadeModels.ToList();
        }

        public List<StructureModel> GetStructures()
        {
            if (StructureManager.regions == null)
            {
                return new List<StructureModel>();
            }

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
                    playerInventory.Add(new ItemModel(item, Utils.FindItem(item.item.id)!));
                }
            }
            return playerInventory;
        }
    }
}
