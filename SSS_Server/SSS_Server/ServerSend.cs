using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SSS_Server.Packet;

namespace SSS_Server
{
    class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet) 
        {
            _packet.WriteLength();
            Server.clientList[_toClient].tcp.SendData(_packet);
        }

        private static void SendTCPDataAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 0; i < Server.MaxPlayers; i++)
            {
                Server.clientList[i].tcp.SendData(_packet);
            }
        }

        private static void SendUDPData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Server.clientList[_toClient].udp.SendData(_packet);
        }

        private static void SendUDPDataAll(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 0; i < Server.MaxPlayers; i++)
            {
                Server.clientList[i].udp.SendData(_packet);
            }
        }



        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        public static void SpawnPlayer(int _toClient, Player _player)
        {
            using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                _packet.Write(_player.id);
                _packet.Write(_player.username);
                _packet.Write(_player.position);
                _packet.Write(_player.rotation);

                SendTCPData(_toClient, _packet);
            }
        }
    }
}
