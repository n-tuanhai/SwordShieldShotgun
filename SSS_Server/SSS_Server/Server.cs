using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using static SSS_Server.Packet;

namespace SSS_Server
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> clientList = new Dictionary<int, Client>();

        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static Socket listener;

        public static void Start(int _maxPlayer, int _port)
        {
            MaxPlayers = _maxPlayer;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            // Create a TCP/IP socket.  
            listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(new IPEndPoint(IPAddress.Any, Port));
                listener.Listen(100);

                // Start an asynchronous socket to listen for connections.  
                Console.WriteLine($"Server started on {Port}.");
                listener.BeginAccept(
                    new AsyncCallback(TCPConnectCallback),
                    null);
                Console.WriteLine("Waiting for a connection...");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            Socket _client = listener.EndAccept(_result);
            listener.BeginAccept(
                    new AsyncCallback(TCPConnectCallback),
                    null);

            Console.WriteLine($"Incoming connection from {_client.RemoteEndPoint}");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clientList[i].tcp.socket == null)
                {
                    clientList[i].tcp.Connect(_client);
                    return;
                }
            }

            Console.WriteLine($"{_client.RemoteEndPoint} Failed to connect: Server full!");
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clientList.Add(i, new Client(i));
            }
            packetHandlers = new Dictionary<int, PacketHandler>()
                {
                    { (int)ClientPackets.welcome, ServerHandler.WelcomeReceived }
                };
        }
    }
}
