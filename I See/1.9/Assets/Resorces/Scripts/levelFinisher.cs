using UnityEngine;
using System.Collections;

public class levelFinisher : MonoBehaviour
{
    public string nextLevel = "";

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Network.peerType == NetworkPeerType.Disconnected)
            {
                Application.LoadLevel(nextLevel);
            }
            else
            {
                networkView.RPC("LoadAndSync", RPCMode.AllBuffered);
            }
        }
    }

    private void LoadAndSync()
    {
        Application.LoadLevel(nextLevel);
    }
}