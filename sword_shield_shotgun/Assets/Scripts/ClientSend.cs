using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SSS_Client.Packet;

namespace SSS_Client
{
    public class ClientSend : MonoBehaviour
    {
        private static void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.instance.tcp.SendData(_packet);
        }

        private static void SendUDPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.instance.udp.SendData(_packet);
        }

        public static void WelcomeReceived()
        {
            using (Packet _packet = new Packet((int)ClientPackets.welcome))
            {
                _packet.Write(Client.instance.id);
                _packet.Write(UIManager.instance.userName.text);

                SendTCPData(_packet);
            }
        }


        public static void PlayerMovement(bool[] _inputs)
        {
            using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
            {
                _packet.Write(_inputs.Length);
                foreach (bool _input in _inputs)
                {
                    _packet.Write(_input);
                }
                _packet.Write(GameManager.players[Client.instance.id].transform.rotation);

                SendUDPData(_packet);
            }
        }

        public static void PlayerShoot(Vector3 _facing)
        {
            using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
            {
                _packet.Write(_facing);

                SendTCPData(_packet);
            }
        }
    }

}