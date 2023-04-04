using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomButtonController : MonoBehaviour
{
    ScaleLerpingAnimator buttonScaleLerpingAnimator;

    // Start is called before the first frame update
    void Start()
    {
        buttonScaleLerpingAnimator = GetComponent<ScaleLerpingAnimator>();
    }

    public void OnClick()
    {
        buttonScaleLerpingAnimator.DoScaleAnimator();
    }
}
