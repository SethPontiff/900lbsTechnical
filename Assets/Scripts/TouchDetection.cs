using UnityEngine;
using System.Collections;

public class TouchDetection : MonoBehaviour
{
    public GameObject ARObj;


    #region Scaling
    Vector3 StartingScale;
    float PinchDistanceStart;

    void Scale()
    {
        //StartingScale = ARObj.transform.localScale;

        if (Input.touchCount == 2)
        {
            var Finger1 = Input.GetTouch(0); //Register and store when both fingers are on the screen.
            var Finger2 = Input.GetTouch(1);


            if (Finger1.phase == TouchPhase.Ended ||Finger2.phase == TouchPhase.Ended)
            {
                return; //If the fingers aren't on the screen, return.
            }

            if (Finger1.phase == TouchPhase.Began || Finger2.phase == TouchPhase.Began)
            {               
                PinchDistanceStart = Vector2.Distance(Finger1.position, Finger2.position); //Find start distance when fingers are on the screen.
                StartingScale = ARObj.transform.localScale; //The starting scale of the object before changes ar made to its scale.
            }
            else
            {
                var PinchDistanceCurrent = Vector2.Distance(Finger1.position, Finger2.position); 

                if(Mathf.Approximately(PinchDistanceStart, 0))
                {
                    return; // Basically make it to where nothing happens if the movement is too small.
                }

                var Scaling = PinchDistanceCurrent / PinchDistanceStart; //Divide the current distance by the initial distance
                ARObj.transform.localScale = StartingScale * Scaling;   //Using the result, multiply by the starting scale of the object, which later gets saved as the new starting scale for when this gets called again.
            }
        }
    }
    #endregion


    #region Movement
    private float speed = 0.0007f;
    bool isMoving = false;
    void Movement()
    {
        var touch = Input.GetTouch(0);

        if (Input.touchCount == 0)
        {
            //isMoving = false;
        }
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;
        if (Physics.Raycast(ray , out hit))
        {
            if (hit.collider.gameObject && !isRotating)
            {
                ARObj = hit.collider.gameObject; //If raycast hits the object, set a bool to true that enables us to actually it.
                isMoving = true;
            }
        }
        if (touch.phase == TouchPhase.Ended)
        {
            isMoving = false; //Set the bool back to false when we aren't touching the screen anymore.
        }

        if (Input.touchCount == 1 && isMoving)
        {
            if (touch.phase == TouchPhase.Moved)
            {
                //while the bool is true and a finger is moving on the screen, adjust the position of the object at a specific speed.
                    transform.position = new Vector3(
                    transform.position.x + touch.deltaPosition.x * speed, transform.position.y,
                    transform.position.z + touch.deltaPosition.y * speed);
            }
        }
    }
    #endregion
    

    #region Rotation
    public float rotationSpeed = 50f;   
    bool isRotating = false;

    void Rotation()
    {
        var touch = Input.GetTouch(0);

        if (Input.touchCount == 1 && !isMoving)
        {
            isRotating = true;
            float rotationX = Input.GetAxis("Mouse X") * rotationSpeed; //Get the horizonal and vertical input.
            float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed;
            rotationX *= Time.deltaTime; //Multiply by deltaTime to make it run at this every second not every frame.
            rotationY *= Time.deltaTime;

            Camera camera = Camera.main;

            Vector3 right = Vector3.Cross(camera.transform.up, transform.position - camera.transform.position); //Use Cross to find what the right vector is by putting in the cameras top/up vector (the green arrow) and the result of subtracting the cameras current position from the objects position
            Vector3 up = Vector3.Cross(transform.position - camera.transform.position, right); //Same idea here, but with the second part of the previous one being the first vector, and the vector we just calculated above as the second one.

            transform.rotation = Quaternion.AngleAxis(rotationX, right) * transform.rotation; //Enables rotation on the X axis multiplied by the current rotation.
            transform.rotation = Quaternion.AngleAxis(rotationY, up) * transform.rotation; //Enables rotation on the Y axis multiplied by the current rotation
        }
        if (touch.phase == TouchPhase.Ended)
        {
            isRotating = false;
        }
    }
    #endregion

    #region Coloring
    //Just assign a random color everytime this gets called. I was unsure how I could do this with just touch input, considering the controls used for the previous functionality, so I assigned it to a button
    //Could also maybe make it to where everytime the object gets tapped it gets a random color?
    public void RandomColor()
    {
        ARObj.GetComponent<MeshRenderer>().material.color = GetRandomColor();
    }

    Color GetRandomColor() => Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

    #endregion

    void FixedUpdate()
    {
        Movement();
        Rotation();
        Scale();
    }
}