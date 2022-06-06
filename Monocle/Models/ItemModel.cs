using Monocle.Api;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle.Models
{
    internal class ItemModel
    {
        public string Name { get; set; }
        public int Amount { get; set; }
        public ushort Id { get; set; }
        public string InstanceId { get; set; }
        public byte Durability { get; set; }
        public ItemType Type { get; set; }
        public ItemRarity Rarity { get; set; }
        public WeaponAttachmentsModel? WeaponAttachments { get; set; }

        public ItemModel(ItemJar item, ItemAsset asset)
        {
            Amount = item.item.amount;
            Name = asset.FriendlyName;
            Durability = item.item.durability;
            Id = item.item.id;
            Type = (ItemType)asset.type;
            Rarity = (ItemRarity)asset.rarity;
            InstanceId = "TODO";

            if (asset.type == EItemType.GUN)
            {
                WeaponAttachments = WeaponAttachmentsModel.FromMetadata(item.item.metadata);
            }
        }

        public ItemModel(ItemAsset item, byte? durability)
        {
            Amount = item.amount;
            Name = item.FriendlyName;
            Durability = durability ?? 100;
            Id = item.id;
            Type = (ItemType)item.type;
            Rarity = (ItemRarity)item.rarity;
            InstanceId = "TODO";
        }
    }

    // Reference: https://steamcommunity.com/sharedfiles/filedetails/?id=2184421464
    internal class WeaponAttachmentsModel
    {
        public ItemModel? Sight { get; set; }
        public ItemModel? Tactical { get; set; }
        public ItemModel? Grip { get; set; }
        public ItemModel? Barrel { get; set; }
        public ItemModel? Ammo { get; set; }
        public SwitchState TacticalStatus { get; set; }
        public FireMode FireMode { get; set; }
        public uint CurrentAmmo { get; set; }

        public static WeaponAttachmentsModel FromMetadata(byte[] metadata)
        {
            var sight = Utils.FindItem(FromDualByte(metadata[0], metadata[1]));
            var sightDurability = metadata[13];

            var tactical = Utils.FindItem(FromDualByte(metadata[2], metadata[3]));
            var tacticalDurability = metadata[14];

            var grip = Utils.FindItem(FromDualByte(metadata[4], metadata[5]));
            var gripDurability = metadata[15];

            var barrel = Utils.FindItem(FromDualByte(metadata[6], metadata[7]));
            var barrelDurability = metadata[16];

            var ammo = Utils.FindItem(FromDualByte(metadata[8], metadata[9]));
            var ammoDurability = metadata[17];

            var currentAmmo = metadata[10];
            var fireMode = metadata[11];
            var tacticalStatus = metadata[12];

            return new WeaponAttachmentsModel
            {
                Sight = MakeItem(sight, sightDurability),
                Tactical = MakeItem(tactical, tacticalDurability),
                Grip = MakeItem(grip, gripDurability),
                Barrel = MakeItem(barrel, barrelDurability),
                Ammo = MakeItem(ammo, ammoDurability),
                CurrentAmmo = currentAmmo,
                FireMode = ParseFireMode(fireMode),
                TacticalStatus = ParseSwitchState(tacticalStatus),
            };
        }

        private static FireMode ParseFireMode(byte b)
        {
            return b switch
            {
                0 => FireMode.Safety,
                1 => FireMode.SemiAuto,
                2 => FireMode.FullAuto,
                3 => FireMode.Burst,
                _ => FireMode.Unknown
            };
        }

        private static SwitchState ParseSwitchState(byte b)
        {
            return b switch
            {
                0 => SwitchState.Off,
                1 => SwitchState.On,
                _ => SwitchState.Unknown,
            };
        }

        private static ItemModel? MakeItem(ItemAsset? asset, byte itemDurability)
        {
            return asset != null ? new ItemModel(asset, itemDurability) : null;
        }

        private static ushort FromDualByte(byte byte1, byte byte2)
        {
            return (ushort)((byte2 * 256) + byte1);
        }
    }
}
