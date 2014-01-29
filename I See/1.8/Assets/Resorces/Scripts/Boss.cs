using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    public GameObject projectile;
    public float projectileAcceleration = 5.0f;

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
            if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    GameObject p = (GameObject)Instantiate(projectile, transform.position + new Vector3(0, 0, -3.2f), Quaternion.identity);
                    p.transform.LookAt(player.transform.position);
                    p.rigidbody.AddForce(p.transform.forward * projectileAcceleration * 100, ForceMode.Acceleration);
                    yield return new WaitForSeconds(Random.Range(0.8f, 2.5f));
                }
            }
        }
        yield return new WaitForEndOfFrame();
        StartCoroutine(Shoot());
    }
}