using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadedImage : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Cube")
        {
            gameObject.SetActive(false);
        }
    }
   
}
