using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    [SerializeField] bool isTilting;
    [SerializeField] float tiltAmount;
    [SerializeField] LayerMask layer;

    float rotX, rotY;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        if (isTilting)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 50, layer))
            {
                float tiltSpeed = 0.1f * Mathf.Pow(2, -Vector3.Distance(Vector3.zero, hit.point));

                rotX -= Input.GetAxis("Mouse Y") * tiltSpeed;
                rotY += Input.GetAxis("Mouse X") * tiltSpeed;

                rotX = Mathf.Clamp(rotX, -tiltAmount, tiltAmount);
                rotY = Mathf.Clamp(rotY, -tiltAmount, tiltAmount);

                transform.eulerAngles = new Vector3(rotX, rotY, 0);
            }
        }
    }
}
