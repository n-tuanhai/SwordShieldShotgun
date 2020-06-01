using System.Collections;
using System;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using static Packet;

public class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Client> clientList = new Dictionary<int, Client>();

    public delegate void PacketHandler(int _fromClient, Packet _packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static Socket listener;
    private static UdpClient udpListener;

    public static void Start(int _maxPlayer, int _port)
    {
        MaxPlayers = _maxPlayer;
        Port = _port;

        Debug.Log("Starting server...");
        InitializeServerData();

        // Create a TCP/IP socket.  
        listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        // Bind the socket to the local endpoint and listen for incoming connections.  
        try
        {
            listener.Bind(new IPEndPoint(IPAddress.Any, Port));
            listener.Listen(100);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            // Start an asynchronous socket to listen for connections.  
            Debug.Log($"Server started on {Port}.");
            listener.BeginAccept(
                new AsyncCallback(TCPConnectCallback),
                null);
            Debug.Log("Waiting for a connection...");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private static void TCPConnectCallback(IAsyncResult _result)
    {
        Socket _client = listener.EndAccept(_result);
        listener.BeginAccept(
                new AsyncCallback(TCPConnectCallback),
                null);

        Debug.Log($"Incoming connection from {_client.RemoteEndPoint}");

        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clientList[i].tcp.socket == null)
            {
                clientList[i].tcp.Connect(_client);
                return;
            }
        }

        Debug.Log($"{_client.RemoteEndPoint} Failed to connect: Server full!");
    }

    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            if (_data.Length < 4)
            {
                return;
            }

            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0)
                {
                    return;
                }

                if (clientList[_clientId].udp.endPoint == null)
                {
                    clientList[_clientId].udp.Connect(_clientEndPoint);
                    return;
                }

                if (clientList[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    clientList[_clientId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error receiving UDP data: {_ex}");
        }
    }

    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
        }
    }

    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clientList.Add(i, new Client(i));
        }
        packetHandlers = new Dictionary<int, PacketHandler>()
                {
                    { (int)ClientPackets.welcome, ServerHandler.WelcomeReceived },
                    { (int)ClientPackets.playerMovement, ServerHandler.PlayerMovement },
                    { (int)ClientPackets.playerShoot, ServerHandler.PlayerShoot }
                };
    }

    public static void Stop()
    {
        listener.Close();
        udpListener.Close();
    }
}
