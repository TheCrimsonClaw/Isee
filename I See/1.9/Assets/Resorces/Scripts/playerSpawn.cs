using UnityEngine;
using System.Collections;

public class playerSpawn : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Start()
    {
        if (Network.peerType == NetworkPeerType.Disconnected)
            Instantiate(playerPrefab, transform.position, Quaternion.identity);
        else
            Network.Instantiate(playerPrefab, transform.position, Quaternion.identity, 0);
    }
}