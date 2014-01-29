using UnityEngine;
using System.Collections;

public class powerUps : MonoBehaviour
{
    public Player.PowerTypes type;
    public AudioClip powerPickup;

    private void Start()
    {
        if (!gameObject.GetComponent<AudioSource>())
        {
            gameObject.AddComponent<AudioSource>();
            audio.playOnAwake = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<Player>().inventory.IndexOf(type) == -1)
            {
                other.gameObject.GetComponent<Player>().inventory.Add(type);
                audio.pitch = Random.Range(0.9f, 1.05f);
                audio.clip = powerPickup;
                audio.Play();
            }
        }
    }
}