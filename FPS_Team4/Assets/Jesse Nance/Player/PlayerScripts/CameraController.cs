using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVertMin, LockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        cameraRotation();
    }

    void cameraRotation()
    {
        //Input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        if (!invertY)
            rotX -= mouseY;
        else
            rotX += mouseY;

        //Camera clamp
        rotX = Mathf.Clamp(rotX, lockVertMin, LockVertMax);

        //Camera pivot on X-Axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //Rotate player on Y-Axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
