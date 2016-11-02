using UnityEngine;
using System.Collections;

public class GroundEnemy : MonoBehaviour {

    NavMeshAgent agent;
    GameObject conductor;
    public float health;

	// Use this for initialization
	void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        conductor = GameObject.FindGameObjectWithTag("conductor");
        agent.SetDestination(conductor.transform.position);
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (health <= 0)
        {
            Destroy(gameObject);
        }
	}
}
