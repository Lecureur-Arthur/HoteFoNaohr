using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBuildMod : MonoBehaviour
{

    public GameObject slider_buildMod;

    private bool openSlide = false;
    private Animator anim;


    private void Start()
    {
        anim = slider_buildMod.GetComponent<Animator>();
    }

    public void clickBtn()
    {
        switch (openSlide)
        {
            case false:
                openSlide = true;
                anim.SetBool("nearby", true);
                break;
            case true:
                openSlide = false;
                anim.SetBool("nearby", false);
                break;
        }
    }
}
