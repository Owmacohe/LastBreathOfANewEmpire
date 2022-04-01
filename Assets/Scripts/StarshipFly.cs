using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarshipFly : MonoBehaviour
{
    [SerializeField] float speed = 0.01f;

    GameController controller;

    private void Start()
    {
        controller = FindObjectOfType<GameController>();
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, controller.homePlanet.transform.position) <= 0.1f)
        {
            controller.StartCoversation();
            Destroy(gameObject);
        }
        else
        {
            transform.LookAt(controller.homePlanet.transform);
            transform.localPosition += transform.forward * speed;
        }
    }
}
