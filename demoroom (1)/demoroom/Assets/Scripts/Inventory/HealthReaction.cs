using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthReaction : MonoBehaviour
{
    public float playerHealth;
    //public Signal HealthSignal;


    public void Use(int amountToIncrease)
    {
        playerHealth/*.RuntimeValue*/ += amountToIncrease;
        //healthSignal.Raise();
    }
}
