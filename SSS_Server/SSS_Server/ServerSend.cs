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

        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }
    }
}
