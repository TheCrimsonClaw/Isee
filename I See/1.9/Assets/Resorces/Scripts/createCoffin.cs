using UnityEngine;
using System.Collections;

public class createCoffin : MonoBehaviour
{
    public GameObject blood;
    public float increaseHeight = 2.0f;

    private IEnumerator Start()
    {
        increaseHeight += transform.position.y;
        yield return new WaitForSeconds(2.0f);
        for (int x = 0; x < 5; x++ )
        {
            Instantiate(blood, transform.position + new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)), Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void Update()
    {
        if (transform.position.y < increaseHeight)
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, increaseHeight, transform.position.z), Time.deltaTime * 2);
    }
}