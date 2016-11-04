using UnityEngine;
using System.Collections;

public class GroundEnemy : MonoBehaviour {

    NavMeshAgent agent;
    GameObject conductor;
    public float health;
    public float xpPayout;
    int layerMask;

	// Use this for initialization
	void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        conductor = GameObject.FindGameObjectWithTag("conductor");
        agent.SetDestination(conductor.transform.position);
        layerMask = 1 << 8;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if (health <= 0)
        {
            WeDied();
        }
	}

    public void TakeDamage(float damageTaken)
    {
        health -= damageTaken;
    }

    void WeDied()
    {
        // when we die, give XP split between all towers nearby
        Collider[] towersInRange = Physics.OverlapSphere(transform.position, 20, layerMask);
        float splitXPPayout = xpPayout / towersInRange.Length;
        foreach (Collider col in towersInRange)
        {
            col.SendMessage("AddExperience", splitXPPayout);
        }
        // remove ourselfs from the number of alive enemies
        GameManager.numberOfEnemiesAlive--;
        // now we die
        Destroy(gameObject);
    }
}
