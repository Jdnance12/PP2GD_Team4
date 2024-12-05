using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

        //tie mouseY input to rotX
        if (!invertY)
        {
            rotX -= mouseY;
        }
        else
        {
            rotX += mouseY;
        }

        //clamp cam x rot
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        //rotate cam on x-axis
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //rotate player on y-axis
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
