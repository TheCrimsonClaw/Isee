using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    public GameObject projectile;
    public float projectileAcceleration = 5.0f;
    public float Health = 10;
    public AudioClip hit, death;

    private GameObject player;

    private void Start()
    {
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, 15))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    GameObject p = (GameObject)Instantiate(projectile, transform.position + new Vector3(0, 0, -3.2f), Quaternion.identity);
                    p.transform.LookAt(player.transform.position);
                    p.rigidbody.AddForce(p.transform.forward * projectileAcceleration * 100, ForceMode.Acceleration);
                    
                    yield return new WaitForSeconds(Random.Range(0.95f, 2.5f));
                }
            }
        }


        if (Health <= 0)
        {
            //End Scene
            GameObject g = new GameObject();
            g.AddComponent<AudioSource>();
            g.audio.clip = death;
            g.audio.Play();

            StartCoroutine(player.GetComponent<Player>().FinishGame(g.audio.clip.length));

            Application.LoadLevel("end");

            Destroy(gameObject);
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(Shoot());
    }

    internal void decreaseDamage(int p)
    {
        audio.clip = hit;
        audio.Play();
        Health -= p;
    }
}