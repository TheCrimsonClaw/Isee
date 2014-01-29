using UnityEngine;
using System.Collections;

public class showAnddistroy : MonoBehaviour 
{
	public float waitingTime = 6;
	
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(waitingTime);
		Destroy(gameObject);
	}
}
