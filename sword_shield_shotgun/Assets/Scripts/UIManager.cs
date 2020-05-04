using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SSS_Client
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        public GameObject startMenu;
        public InputField userName;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(this);
            }
        }

        public void ConnectToServer()
        {
            startMenu.SetActive(false);
            userName.interactable = false;
            Client.instance.ConnectToServer();
        }
    } 
}
