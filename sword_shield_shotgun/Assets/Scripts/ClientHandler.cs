using System.Collections;
using System.Collections.Generic;
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
        }
    }

}