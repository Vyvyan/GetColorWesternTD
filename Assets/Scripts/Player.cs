using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    Animator anim;
    public float playerSpeed;
    float startingSpeed;
    bool isWalking;
    bool isAiming;
    public GameObject laserSight;
    Renderer laserSightRend;
    public GameObject gunTip;
    bool canFireGun;

	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
        startingSpeed = playerSpeed;
        laserSightRend = laserSight.GetComponentInChildren<Renderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	    if (anim.enabled)
        {
            // AIMING
            if (isAiming)
            {
                RaycastHit laserSightHit;
                Ray laserRay = new Ray(gunTip.transform.position, transform.forward);

                // if we are aiming, enable the laser
                if (!laserSightRend.enabled) { laserSightRend.enabled = true; }

                // if the laser sight hits a wall, then change the line end point, if we don't hit anything, make it end way off in the distance
                if (Physics.Raycast(laserRay, out laserSightHit, 1000))
                {
                    // stretch to the end point
                    laserSight.transform.localScale = new Vector3(laserSight.transform.localScale.x, laserSight.transform.localScale.y, laserSightHit.distance);
                }
                

                // SHOOTING
                if (Input.GetAxis("Fire1") > 0)
                {
                    if (canFireGun)
                    {
                        FireGun();
                    }
                }

                // look at our mouse
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 100))
                {
                    transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
                }

                anim.SetBool("IsMoving", false);
            }
            else
            {
                // if we aren't aiming, disable the line renderer
                if (laserSightRend.enabled) { laserSightRend.enabled = false; }

                // MOVING
                // animation stuff
                if (movement.magnitude > .01f)
                {
                    transform.rotation = Quaternion.LookRotation(movement.normalized);
                    anim.SetBool("IsMoving", true);
                    if (isWalking)
                    {
                        anim.SetBool("IsWalking", true);
                    }
                    else
                    {
                        anim.SetBool("IsWalking", false);
                    }
                }
                else
                {
                    anim.SetBool("IsMoving", false);
                }

                transform.Translate(movement.normalized * (playerSpeed * Time.deltaTime), Space.World);
            }
        }

        // walking
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isWalking = true;
            playerSpeed = startingSpeed * .25f;
        }
        else
        {
            isWalking = false;
            playerSpeed = startingSpeed;
        }

        // aiming hold
        if (Input.GetAxis("Fire2") > 0)
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }

        // if the fire button is up
        if (Input.GetAxis("Fire1") == 0)
        {
            // we can fire
            canFireGun = true;
        }
	}
    
    void FireGun()
    {
        canFireGun = false;
        Ray bulletRay = new Ray(gunTip.transform.position, gunTip.transform.forward);
        RaycastHit bulletHit;

        if (Physics.Raycast(bulletRay, out bulletHit, 500))
        {
            Debug.Log("we shot " + bulletHit.collider.name.ToString());
        }
    }   
}
