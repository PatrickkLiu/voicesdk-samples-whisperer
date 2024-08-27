using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RenderChange : MonoBehaviour
{
    public GameObject renderScreen;
    //public GameObject renderScreen_Right;
    public bool ringScene = false;
    [SerializeField] AudioSource crossFadeSound;
    [SerializeField] VisualEffect displayPointcloud;

    public void FadeIn()
    {
        renderScreen.GetComponent<Animation>().Play("FadeIn");
        //renderScreen_Right.GetComponent<Animation>().Play("FadeIn");
        crossFadeSound.Play();
        ringScene = true;
    }

    public void FadeOut()
    {
        renderScreen.GetComponent<Animation>().Play("FadeOut");
        //renderScreen_Right.GetComponent<Animation>().Play("FadeOut");
        crossFadeSound.Play();
        displayPointcloud.SendEvent("Stop");
        ringScene = false;
    }

    void Update()
    {
        if(Input.GetKeyDown("z"))
        {
            FadeIn();
        }
        if (Input.GetKeyDown("x"))
        {
            FadeOut();
        }


    }

}
