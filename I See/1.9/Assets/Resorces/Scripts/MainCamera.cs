using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour
{
    public GameObject[] target;
    public float cameraSpeed = 5.0f;
    public float cameraHeight = 10.0f;

    private void Start()
    {
        if (Network.peerType != NetworkPeerType.Disconnected && !networkView.isMine)
            Destroy(gameObject);
    }

    private void Update()
    {
        Vector3 cameraPos = Vector3.zero;
        for (int x = 0; x < target.Length; x++)
        {
            cameraPos.x += target[x].transform.position.x;
            cameraPos.z += target[x].transform.position.z;
        }
        cameraPos.x = cameraPos.x / target.Length;
        cameraPos.z = cameraPos.z / target.Length;


        cameraPos.y = cameraHeight;
        transform.position = Vector3.Lerp(transform.position, cameraPos, Time.deltaTime * cameraSpeed);
    }
}