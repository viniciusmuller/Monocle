using Fleck;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Monocle.Services
{
    internal class Server
    {
        WebSocketServer SocketServer { get; set; }

        public void Start(string ip, int port)
        {
            var host = $"ws://{IPAddress.Parse(ip)}:{port}";
            SocketServer = new WebSocketServer(host);
            Logger.Log($"Starting WebSocket server at {host}");
            SocketServer.Start(socket =>
            {
                socket.OnOpen = () => Logger.Log("New connection");
                socket.OnClose = () => Logger.Log("Closed connection");
                socket.OnMessage = message => socket.Send(message);
            });
        }

        public void Stop()
        {
            Logger.Log($"Stopping server...");
            SocketServer.Dispose();
        }
    }
}
