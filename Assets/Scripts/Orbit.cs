using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform parent;
    public float speed;

    GameObject rotator;
    SpaceSpawner spawner;

    private void Start()
    {
        if (parent != null)
        {
            rotator = new GameObject();
            rotator.transform.SetParent(parent);
            rotator.transform.localPosition = Vector3.zero;

            gameObject.transform.SetParent(rotator.transform);

            spawner = FindObjectOfType<SpaceSpawner>();
        }
    }

    private void FixedUpdate()
    {
        if (rotator != null && speed >= 0.0003f)
        {
            rotator.transform.Rotate(Vector3.up, speed * spawner.orbitFactor);
        }
    }
}
