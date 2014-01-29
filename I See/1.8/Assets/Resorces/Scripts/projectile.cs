using UnityEngine;
using System.Collections;

public class projectile : MonoBehaviour
{
    public bool playerSize = false;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!playerSize)
        {
            if (other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<Player>().health--;
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.gameObject.tag == "Enemey")
            {
                Destroy(gameObject);
            }
        }
    }
}