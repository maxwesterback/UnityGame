using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpDialogue : MonoBehaviour
{
    public GameObject popUpBox;
    public Animator anim;
    public TMP_Text popUpText;
    public TMP_Text popUpDescription;
    public bool isFrozen = false;

   
    public void PopUp(string text, string desc)
    {
        popUpBox.SetActive(true);
        popUpText.text = text;
        popUpDescription.text = desc;
        anim.SetTrigger("pop");
        Pause();
    }

    public void Resume()
    {
        popUpBox.SetActive(false);

        Time.timeScale = 1f;
        isFrozen = false;
    }

    public void Pause()
    {
        popUpBox.SetActive(true);
        Time.timeScale = 0f;
        isFrozen = true;
        
    }

}



