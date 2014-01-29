using UnityEngine;
using System.Collections;

public class projectile : MonoBehaviour
{
    public bool playerSize = false;

    private IEnumerator Start()
    {
        if (playerSize)
        {
            audio.pitch = Random.Range(2.0f, 2.5f);
        }
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
                StartCoroutine(createSound(other.collider.gameObject.GetComponent<Player>().Screams[Random.Range(0, other.collider.gameObject.GetComponent<Player>().Screams.Length)]));
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.gameObject.tag == "Enemey")
            {
                Destroy(other.collider.gameObject);
                Destroy(gameObject);
            }
            else if (other.gameObject.tag == "Boss")
            {
                other.collider.gameObject.GetComponent<Boss>().decreaseDamage(1);
            }
        }
    }

    public IEnumerator createSound(AudioClip clip)
    {
        GameObject g = new GameObject();
        g.AddComponent<AudioSource>();
        g.audio.pitch = Random.Range(0.95f, 1.05f);
        g.audio.clip = clip;
        g.audio.Play();

        yield return new WaitForSeconds(clip.length);

        Destroy(g);
    }
}