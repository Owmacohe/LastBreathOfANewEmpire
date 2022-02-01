using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarshipFly : MonoBehaviour
{
    public float speed = 0.01f;

    private GameController controller;

    private void Start()
    {
        controller = FindObjectOfType<GameController>();
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, controller.homePlanet.transform.position) <= 0.1f)
        {
            controller.startCoversation();
            Destroy(gameObject);
        }
        else
        {
            transform.localPosition += transform.forward * speed;
        }
    }
}
