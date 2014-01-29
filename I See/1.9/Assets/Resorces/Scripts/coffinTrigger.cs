using UnityEngine;
using System.Collections;

public class coffinTrigger : MonoBehaviour
{
    public GameObject coffin;
    public Vector3 spawnPoisition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Instantiate(coffin, spawnPoisition - new Vector3(0, 2, 0), transform.rotation);
            Destroy(gameObject);
        }
    }
}