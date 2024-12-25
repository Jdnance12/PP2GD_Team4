using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{

    [SerializeField] int sensitivity;
    [SerializeField] int lockMin, lockMax;
    float rotX;

    [SerializeField] bool invertY;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CameraRotation();
    }

    void CameraRotation()
    {
        //Input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //Player Rotation
        transform.parent.Rotate(Vector3.up * mouseX);

        //Camera Pivot on X-Axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //Clamp
        rotX = Mathf.Clamp(rotX, lockMin, lockMax);

        if(!invertY)
        {
            rotX -= mouseY;
        }
        else
        {
            rotX += mouseY;
        }
    }
}
