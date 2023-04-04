using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleLerpingAnimator : MonoBehaviour
{
    public Vector3 minScale;
    public Vector3 maxScale;
    public float scalingSpeed;
    public float scalingDruation;

    public void DoScaleAnimator()
    {
        StartCoroutine(ActiveScaleAnimator());
    }

    IEnumerator ActiveScaleAnimator()
    {
            yield return RepeatLerping(minScale, maxScale, scalingDruation);
            yield return RepeatLerping(maxScale, new Vector3(1,1,0), scalingDruation);
    }

    IEnumerator RepeatLerping(Vector3 starScale, Vector3 endScale, float time)
    {
        float t = 0.0f;
        float rate = (1f/time) * scalingSpeed;
        while (t < 1f)
        {
            t += Time.deltaTime * rate;
            transform.localScale = Vector3.Lerp(starScale, endScale, t);
            yield return null;
        }
    }
}
