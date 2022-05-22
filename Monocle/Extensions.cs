using Monocle.Api;
using Monocle.Models;
using Monocle.Services;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Monocle
{
    internal static class Extensions
    {
        public static Position ToPosition(this Vector3 v)
        {
            return new Position
            {
                x = v.x,
                y = v.y,
                z = v.z,
            };
        }

        public static string GetName(this ItemJar item)
        {
            return Assets.find(EAssetType.ITEM, item.item.id).FriendlyName;
        }

        public static ChatMode ToMonocleChatMode(this EChatMode chatMode)
        {
            return chatMode switch
            {
                EChatMode.LOCAL => ChatMode.Local,
                EChatMode.GLOBAL => ChatMode.Global,
                EChatMode.GROUP => ChatMode.Group,
                EChatMode.SAY => ChatMode.Say,
                EChatMode.WELCOME => ChatMode.Welcome,
                _ => throw new ArgumentException($"Invalid chat mode {chatMode}")
            };
        }
    }
}
