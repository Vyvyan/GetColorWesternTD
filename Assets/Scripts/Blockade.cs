using UnityEngine;
using System.Collections;

public class Blockade : MonoBehaviour {

    public OffMeshLink ml_up, ml_down, ml_right, ml_left;
    Vector3 heightOffset;

	// Use this for initialization
	void Start ()
    {
        Ray upRay, downRay, leftRay, rightRay;
        upRay = new Ray(ml_up.transform.position, Vector3.down);
        downRay = new Ray(ml_down.transform.position, Vector3.down);
        leftRay = new Ray(ml_left.transform.position, Vector3.down);
        rightRay = new Ray(ml_right.transform.position, Vector3.down);
        RaycastHit upRayHit, downRayHit, leftRayHit, rightRayHit;
        heightOffset = new Vector3(0, .02f, 0);

        // raycast down from the mesh link object (they spawn 8 units above the blockade)
        if (Physics.Raycast(upRay, out upRayHit, 10))
        {
            // if we hit anything except a blockade
            if (upRayHit.collider.gameObject.tag != "Blockade")
            {
                ml_up.transform.position = upRayHit.point + heightOffset;
            }
            else
            {
                // turn off this link because its on another barricade
                ml_up.activated = false;
                ml_up.gameObject.SetActive(false);
            }
        }

        // raycast down from the mesh link object (they spawn 8 units above the blockade)
        if (Physics.Raycast(downRay, out downRayHit, 10))
        {
            // if we hit anything except a blockade
            if (downRayHit.collider.gameObject.tag != "Blockade")
            {
                ml_down.transform.position = downRayHit.point + heightOffset;
            }
            else
            {
                // turn off this link because its on another barricade
                ml_down.activated = false;
                ml_down.gameObject.SetActive(false);
            }
        }

        // raycast down from the mesh link object (they spawn 8 units above the blockade)
        if (Physics.Raycast(leftRay, out leftRayHit, 10))
        {
            // if we hit anything except a blockade
            if (leftRayHit.collider.gameObject.tag != "Blockade")
            {
                ml_left.transform.position = leftRayHit.point + heightOffset;
            }
            else
            {
                // turn off this link because its on another barricade
                ml_left.activated = false;
                ml_left.gameObject.SetActive(false);
            }
        }

        // raycast down from the mesh link object (they spawn 8 units above the blockade)
        if (Physics.Raycast(rightRay, out rightRayHit, 10))
        {
            // if we hit anything except a blockade
            if (rightRayHit.collider.gameObject.tag != "Blockade")
            {
                ml_right.transform.position = rightRayHit.point + heightOffset;
            }
            else
            {
                // turn off this link because its on another barricade
                ml_right.activated = false;
                ml_right.gameObject.SetActive(false);
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }
}
