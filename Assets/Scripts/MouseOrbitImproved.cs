using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{

    public Transform target;
    public GameObject map;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    private Rigidbody rb;

    float x = 0.0f;
    float y = 0.0f;

    public bool isHoldingRightMouse;

    public bool toggleDown, toggleUp, toggleRight, toggleLeft;
    public float cameraSpeed;
    Camera thisCamera;

    float guiSize, guiSizeHorizontal, guiSizeVertical;

    bool isMouseConfined;

    // Use this for initialization
    void Start()
    {
        // this is our map size, so we can make sure the camera doesn't go outside of the bounds of the map
        map = GameObject.FindGameObjectWithTag("Ground");
        thisCamera = GetComponent<Camera>();

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rb = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        isMouseConfined = true;
        ConfineOrFreeMouse();

        // idk wtf im doing
        if (target)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    void Update()
    {
        /*
        guiSize = Screen.width * .15f;
        guiSizeHorizontal = Screen.width * .05f;
        guiSizeVertical = Screen.height * .10f;
        Rect recdown = new Rect(0, 0, Screen.width, guiSizeVertical);
        Rect recup = new Rect(0, Screen.height - guiSizeVertical, Screen.width, guiSize);
        Rect recleft = new Rect(0, 0, guiSizeHorizontal, Screen.height);
        Rect recright = new Rect(Screen.width - guiSizeHorizontal, 0, guiSize, Screen.height);

        if (recdown.Contains(Input.mousePosition))
        {
            toggleDown = true;
        }
        else
        {
            toggleDown = false;
        }

        if (recup.Contains(Input.mousePosition))
        {
            toggleUp = true;
        }
        else
        {
            toggleUp = false;
        }

        if (recleft.Contains(Input.mousePosition))
        {
            toggleLeft = true;
        }
        else
        {
            toggleLeft = false;
        }

        if (recright.Contains(Input.mousePosition))
        {
            toggleRight = true;
        }
        else
        {
            toggleRight = false;
        }
        */

        Vector2 mousePOS = Input.mousePosition;

       
        // if the mouse is on the left side of the screen
        if (mousePOS.x < Screen.width * .05f)
        {
            toggleLeft = true;
        }
        else
        {
            toggleLeft = false;
        }
        // if the mouse is on the right side of the screen
        if (mousePOS.x > Screen.width * .95f)
        {
            toggleRight = true;
        }
        else
        {
            toggleRight = false;
        }

        // if the mouse is on the up side of the screen
        if (mousePOS.y < Screen.height * .1f)
        {
            toggleDown = true;
        }
        else
        {
            toggleDown = false;
        }
        // if the mouse is on the down side of the screen
        if (mousePOS.y > Screen.height * .9f)
        {
            toggleUp = true;
        }
        else
        {
            toggleUp = false;
        }


        // we can only move the target if we aren't spinning the camera
        if (!isHoldingRightMouse)
        {
            if (toggleDown)
            {
                target.transform.Translate(new Vector3(transform.forward.x, 0, transform.forward.z) * (-cameraSpeed * Time.deltaTime));
            }
            if (toggleUp)
            {
                target.transform.Translate(new Vector3(transform.forward.x, 0, transform.forward.z) * (cameraSpeed * Time.deltaTime));
            }
            if (toggleRight)
            {
                target.transform.Translate(new Vector3(transform.right.x, 0, transform.right.z) * (cameraSpeed * Time.deltaTime));
            }
            if (toggleLeft)
            {
                target.transform.Translate(new Vector3(transform.right.x, 0, transform.right.z) * (-cameraSpeed * Time.deltaTime));
            }
        }

        // confine it so our camera target never goes off the map
        target.transform.position = new Vector3(Mathf.Clamp(target.transform.position.x, -(map.transform.localScale.x / 2), map.transform.localScale.x / 2), 0, Mathf.Clamp(target.transform.position.z, -(map.transform.localScale.z) / 2, map.transform.localScale.z / 2));

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMouseConfined = !isMouseConfined;
            ConfineOrFreeMouse();
        }
    }

    void LateUpdate()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isHoldingRightMouse = true;
        }
        else
        {
            isHoldingRightMouse = false;
        }

        if (isHoldingRightMouse)
        {
            if (target)
            {
                // get rid of the - in front of the x and y speeds below to change which way the camera rotates, this way felt more natural
                x += Input.GetAxis("Mouse X") * -xSpeed * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * -ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);

                Quaternion rotation = Quaternion.Euler(y, x, 0);

                distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
                
                Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
                Vector3 position = rotation * negDistance + target.position;

                transform.rotation = rotation;
                transform.position = position;                
            }
        }
        else
        {
            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    public void ToggleDown(bool truefalse)
    {
        if (truefalse)
        {
            toggleDown = true;
        }
        else
        {
            toggleDown = false;
        }
    }

    public void ToggleUp(bool truefalse)
    {
        if (truefalse)
        {
            toggleUp = true;
        }
        else
        {
            toggleUp = false;
        }
    }

    public void ToggleRight(bool truefalse)
    {
        if (truefalse)
        {
            toggleRight = true;
        }
        else
        {
            toggleRight = false;
        }
    }

    public void ToggleLeft(bool truefalse)
    {
        if (truefalse)
        {
            toggleLeft = true;
        }
        else
        {
            toggleLeft = false;
        }
    }

    void ConfineOrFreeMouse()
    {
        if (isMouseConfined)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
