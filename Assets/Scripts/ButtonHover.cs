using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour
{
    [SerializeField] Sprite def;
    [SerializeField] Sprite hov;

    public void OnHover()
    {
        GetComponentInChildren<Image>().sprite = hov;
    }

    public void OffHover()
    {
        GetComponentInChildren<Image>().sprite = def;
    }
}
