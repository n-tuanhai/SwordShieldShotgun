using System.Collections;
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

        public static void playerPosition(Packet _packet)
        {
            int _id = _packet.ReadInt();
            Vector3 _position = _packet.ReadVector3();
            GameManager.instance.playerPosition(_id, _position);
        }

        public static void playerRotation(Packet _packet)
        {
            int _id = _packet.ReadInt();
            Quaternion _rotation = _packet.ReadQuaternion();
            GameManager.instance.playerRotation(_id, _rotation);
        }

        public static void playerHealth(Packet _packet)
        {
            int _id = _packet.ReadInt();
            int _health = _packet.ReadInt();
            GameManager.instance.playerHealth(_id, _health);
        }

        public static void playerRespawned(Packet _packet)
        {
            int _id = _packet.ReadInt();
            GameManager.instance.playerRespawned(_id);
        }

        public static void playerAttack(Packet _packet)
        {
            int _id = _packet.ReadInt();
            GameManager.instance.playerAttack(_id);
        }

        public static void playerDefense(Packet _packet)
        {
            int _id = _packet.ReadInt();
            GameManager.instance.playerDefense(_id);
        }

        public static void playerDisconnected(Packet _packet)
        {
            int _id = _packet.ReadInt();
            GameManager.instance.playerDisconnected(_id);
        }
    }
}