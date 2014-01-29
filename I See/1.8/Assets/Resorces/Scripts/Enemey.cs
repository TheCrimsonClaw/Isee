using UnityEngine;
using System.Collections;

public class Enemey : MonoBehaviour
{
    public float seeingDistance = 50.0f;
    public float speed = 2.0f;

    private GameObject target;

    private void Start()
    {

    }

    private void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (target == null)
        {
            target = players[Random.Range(0, players.Length)];
            return;
        }

        for (int x = 0; x < players.Length; x++)
        {
            if (players[x].GetComponent<Player>().stunBlast)
                return;
        }
        Ray ray = new Ray(transform.position, target.transform.position - transform.position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, seeingDistance))
        {
            if (hit.collider.gameObject == target)
            {
                transform.LookAt(target.transform.position);
                transform.Translate(Vector3.forward * Time.deltaTime * speed);
                if (Vector3.Distance(transform.position, hit.point) < 3)
                {
                    StartCoroutine(hit.collider.gameObject.GetComponent<Player>().screamCreator(hit.collider.gameObject.GetComponent<Player>().Cross));
                    hit.collider.gameObject.GetComponent<Player>().health--;
                    Destroy(gameObject);
                }
            }
        }

        players = GameObject.FindGameObjectsWithTag("Player");
        target = players[Random.Range(0, players.Length)];
    }
}