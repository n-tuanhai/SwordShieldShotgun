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

        public static void WelcomeReceived()
        {
            using (Packet _packet = new Packet((int)ClientPackets.welcome))
            {
                _packet.Write(Client.instance.id);
                _packet.Write(UIManager.instance.userName.text);

                SendTCPData(_packet);
            }
        }
    }

}