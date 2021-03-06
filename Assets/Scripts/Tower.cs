﻿using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

    public enum TowerType { Rifleman, DualRevolvers, DynamiteThrower};
    public TowerType towerType;
    public enum AttackType { aboveGround, belowGround, both, none};
    public AttackType attackType;
    public float health, damage, range, attackspeed, aoeRange;
    public Projector projector;
    public Transform target;
    float attackTimer;
    public int aboveGroundLayerMask, underGroundLayerMask, combinedLayerMask;
    public GameObject raycastOriginObject;
    LineRenderer lineRend;
    Collider targetCollider;
    public int towerLevel;
    public float towerXP, towerXPToLevelNextLevel;
    public TextMesh levelText;

	// Use this for initialization
	void Start ()
    {
        projector = GetComponentInChildren<Projector>();
        // start with an attack ready to go
        attackTimer = 0;

        lineRend = GetComponentInChildren<LineRenderer>();
        lineRend.enabled = false;

        // we level up at spawn, so we're lvl 1
        LevelUp();

        // layer mask setup;
        aboveGroundLayerMask = 1 << 9;
        underGroundLayerMask = 1 << 10;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (attackType == AttackType.aboveGround)
        {
            combinedLayerMask = aboveGroundLayerMask;
        }
        else if (attackType == AttackType.belowGround)
        {
            combinedLayerMask = underGroundLayerMask;
        }
        else if (attackType == AttackType.both)
        {
            combinedLayerMask = aboveGroundLayerMask | underGroundLayerMask;
        }
        else if (attackType == AttackType.none)
        {
            combinedLayerMask = 0;
        }


        // make sure the projector shows the right range (idk why it's divided by 2, it just works)
        projector.orthographicSize = range / 2;

        // if we have a target
        if (target)
        {
            // if we're out of range of our target, get a new one!
            // or if the shot is blocked, get a new target
            // find the direction between the tower and the target, then get the distance (this is more efficient than other methods)
            if (Vector3.Distance(transform.position, target.position) > range * .52f)
            {
                // if the enemy is out of range, then it's not our target anymore
                target = null;
            }
            else
            {
                // FACE OUR TARGET
                transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

                // we should attack if we have a target in range
                if (attackTimer <= 0)
                {
                    Attack();
                    attackTimer = attackspeed;
                }
            }

            // stupid, shut up, putting another check here for reasons, because it can be nulled above
            if (target)
            {
                Ray tempRay;
                RaycastHit hit;
                Vector3 directionToTarget = target.transform.position - raycastOriginObject.transform.position;
                tempRay = new Ray(raycastOriginObject.transform.position, directionToTarget);
                if (Physics.Raycast(tempRay, out hit, Mathf.Infinity))
                {
                    if (hit.collider != targetCollider)
                    {
                        target = null;
                    }
                }
            }
        }

        // reset our tower to fire again
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
	}

    void FixedUpdate()
    {
        // if we dont' have a target, we need to get one, unless we can't attack
        if (attackType != AttackType.none)
        {
            if (!target)
            {
                Collider[] enemiesInRange = Physics.OverlapSphere(gameObject.transform.position, range / 2, combinedLayerMask);
                target = GetClosestEnemy(enemiesInRange);
            }
        }
    }

    Transform GetClosestEnemy(Collider[] enemies)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        RaycastHit hit;
        Ray tempRay;
        foreach (Collider potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - raycastOriginObject.transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            tempRay = new Ray(raycastOriginObject.transform.position, directionToTarget);
            if (dSqrToTarget < closestDistanceSqr)
            {
                // ray cast to the enemy, so we can't shoot through walls
                if (Physics.Raycast(tempRay, out hit, Mathf.Infinity))
                {
                    if (hit.collider.tag == "enemy")
                    {
                        closestDistanceSqr = dSqrToTarget;
                        bestTarget = potentialTarget.transform;
                        targetCollider = bestTarget.GetComponent<Collider>();
                    }
                }
            }
        }

        if (bestTarget)
        {
            return bestTarget;
        }
        else
        {
            return null;
        }
    }

    void Attack()
    {
        StartCoroutine(ShowDebugLineAttack());

        // aoe attack
        if (towerType == TowerType.DynamiteThrower)
        {
            Collider[] enemiesInExplosionRadius = Physics.OverlapSphere(target.transform.position, aoeRange / 2, combinedLayerMask);
            foreach (Collider col in enemiesInExplosionRadius)
            {
                col.SendMessage("TakeDamage", damage);
            }
        }
        // normal attack
        else
        {
            target.SendMessage("TakeDamage", damage);
        }
    }

    IEnumerator ShowDebugLineAttack()
    {
        lineRend.SetPosition(0, transform.position);
        lineRend.SetPosition(1, target.position);
        lineRend.enabled = true;
        yield return new WaitForSeconds(.1f);
        lineRend.enabled = false;
    }

    public void AddExperience(float xpToAdd)
    {
        towerXP += xpToAdd;
        // problem is, it won't level up twice if it gets a burst of XP that would level it twice, fix this later
        if (towerXP >= towerXPToLevelNextLevel)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        towerLevel++;
        // make the xp requirement based on the level
        towerXPToLevelNextLevel += towerLevel * 100;
        levelText.text = "Level: " + towerLevel.ToString();
    }
}
