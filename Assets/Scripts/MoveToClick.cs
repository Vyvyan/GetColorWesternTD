using UnityEngine;
using System.Collections;

public class MoveToClick : MonoBehaviour {

    NavMeshAgent agent;
    float floatingInAirTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        floatingInAirTimer = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                agent.destination = hit.point;
            }
        }
    }

    void LateUpdate()
    {
        // this sees if we are floating on the second plane, if so, we drop down to the first, and HOPEFULLY we snap to the bottom mesh layer again
        Ray heightRay;
        RaycastHit heightHit;
        heightRay = new Ray(gameObject.transform.position, Vector3.down);

        if (transform.position.y > 4)
        {
            if (Physics.Raycast(heightRay, out heightHit, 10))
            {
                if (heightHit.collider.gameObject.tag == "Ground")
                {
                    Debug.Log(gameObject.name + " is floating about ground");
                    floatingInAirTimer += Time.deltaTime;

                    if (floatingInAirTimer > .25f)
                    {
                        WarpToGround();
                    }
                }
                else
                {
                    floatingInAirTimer = 0;
                }
            }
        }
        else
        {
            floatingInAirTimer = 0;
        }
    }

    void WarpToGround()
    {
        Vector3 whereWeAreMovingTo = agent.destination;
        agent.Stop();
        agent.ResetPath();
        agent.Warp(new Vector3(transform.position.x, 3.5f, transform.position.z));
        agent.destination = whereWeAreMovingTo;
        agent.Resume();
        floatingInAirTimer = 0;


        // THIS WORKS, BUT ISN'T GREAT
        //transform.position = new Vector3(transform.position.x, 3.5f, transform.position.z);
    }
}
