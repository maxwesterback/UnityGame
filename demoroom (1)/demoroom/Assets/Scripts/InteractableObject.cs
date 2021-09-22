using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // Start is called before the first frame update
   public void DoInteraction()
    {
        gameObject.SetActive(false);
    }
    public void DoInteraction2()
    {
        gameObject.SetActive(true);
    }
}
