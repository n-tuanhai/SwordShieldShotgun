﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace SSS_Client
{
    public class ClientHandler : MonoBehaviour
    {
        public static void Welcome(Packet _packet)
        {
            string _msg = _packet.ReadString();
            int _id = _packet.ReadInt();

            Debug.Log($"Message from server: {_msg}");
            Client.instance.id = _id;
            ClientSend.WelcomeReceived();

            Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.LocalEndPoint).Port);
        }

        public static void SpawnPlayer(Packet _packet)
        {
            int _id = _packet.ReadInt();
            string _username = _packet.ReadString();
            Vector3 _position = _packet.ReadVector3();
            Quaternion _rotation = _packet.ReadQuaternion();

            GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
        }

        public static void PlayerPosition(Packet _packet)
        {
            int _id = _packet.ReadInt();
            Vector3 _position = _packet.ReadVector3();

            GameManager.players[_id].transform.position = _position;
        }

        public static void PlayerRotation(Packet _packet)
        {
            int _id = _packet.ReadInt();
            Quaternion _rotation = _packet.ReadQuaternion();

            GameManager.players[_id].transform.rotation = _rotation;
        }

        public static void PlayerDisconnected(Packet _packet)
        {
            int _id = _packet.ReadInt();

            Destroy(GameManager.players[_id].gameObject);
            GameManager.players.Remove(_id);
        }
        public static void PlayerHealth(Packet _packet)
        {
            int _id = _packet.ReadInt();
            float _health = _packet.ReadFloat();

            GameManager.players[_id].SetHealth(_health);
        }

        public static void PlayerRespawned(Packet _packet)
        {
            int _id = _packet.ReadInt();

            GameManager.players[_id].Respawn();
        }
    }
}