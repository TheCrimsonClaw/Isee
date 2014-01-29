using UnityEngine;
using System.Collections;

public class powerUps : MonoBehaviour
{
    public Player.PowerTypes type;

    private void  OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            if (other.gameObject.GetComponent<Player>().inventory.IndexOf(type) == -1)
                other.gameObject.GetComponent<Player>().inventory.Add(type);
    }
}