using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreserveObject : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
