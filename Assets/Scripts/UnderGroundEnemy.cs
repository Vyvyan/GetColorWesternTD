using UnityEngine;
using System.Collections;

public class UnderGroundEnemy : MonoBehaviour
{
    GameObject conductor;
    public float health;
    public float xpPayout;
    public float speed;
    int layerMask;
    Transform target;
    Vector3 targetPositionWithOffset;

    // Use this for initialization
    void Start()
    {
        conductor = GameObject.FindGameObjectWithTag("conductor");
        layerMask = 1 << 8;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            WeDied();
        }

        //face and move towards the conductor, the target is here so we can change what it's hunting
        target = conductor.transform;
        // get the position of the target but only on the x and z
        if (target)
        {
            targetPositionWithOffset = new Vector3(target.position.x, gameObject.transform.position.y, target.position.z);
        }
        // look at target
        transform.LookAt(targetPositionWithOffset);
        // move forward
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
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
