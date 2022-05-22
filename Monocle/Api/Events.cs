using Monocle.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Monocle.Api
{
    abstract class Event { }

    class BaseEvent
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public EventType Type;

        public Event Data;

        public BaseEvent(EventType type, Event data)
        {
            Type = type;
            Data = data;
        }
    }

    class PlayerDeathEvent : Event
    {
        public PlayerModel? Killer;
        public PlayerModel Dead;
        public string Cause;

        public PlayerDeathEvent(PlayerModel dead, PlayerModel? killer, string cause)
        {
            Dead = dead;
            Killer = killer;
            Cause = cause;
        }
    }

    class PlayerMessageEvent : Event
    {
        public PlayerModel Player;
        public string Message;
        public string ColorHex;

        [JsonConverter(typeof(StringEnumConverter))]
        public ChatMode ChatMode;

        public PlayerMessageEvent(PlayerModel player, Color color, EChatMode chatMode, string message)
        {
            Player = player;
            Message = message;
            ChatMode = chatMode.ToMonocleChatMode();
            ColorHex = ColorUtility.ToHtmlStringRGB(color);
        }
    }

    class PlayerJoinOrLeaveEvent : Event
    {
        public PlayerModel Player;
        public DateTimeOffset Time;

        public PlayerJoinOrLeaveEvent(PlayerModel player)
        {
            Player = player;
            Time = DateTimeOffset.UtcNow; // TODO: Fix timezone
        }
    }

    enum ChatMode
    {
        Global,
        Local,
        Group,
        Say,
        Welcome,
    }
}
