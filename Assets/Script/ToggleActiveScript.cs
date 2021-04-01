using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleActiveScript : MonoBehaviour
{
    public void ToggleActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
