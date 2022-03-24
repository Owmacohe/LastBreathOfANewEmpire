using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    [SerializeField] bool isTilting;
    [SerializeField] float tiltAmount;
    [SerializeField] LayerMask layer;

    private void Update()
    {
        if (isTilting)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 50, layer))
            {
                transform.LookAt(hit.point / 2);
                //transform.Rotate(hit.point - Vector3.zero);
            }
        }
    }
}
