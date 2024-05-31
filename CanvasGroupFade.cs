using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasGroupFade : MonoBehaviour
{
    public bool fadeOut = false;
    public float fadeTime=1.5f;
    protected CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        StartCoroutine(Fade());
    }

    private void OnDisable()
    {
        if(fadeOut)
        {
            canvasGroup.alpha = 1f;
        }
        else
        {
            canvasGroup.alpha = 0f;
        }
    }


    public IEnumerator Fade()
    {
        yield return null;
        switch (fadeOut)
        {
            case false:
                canvasGroup.alpha = 0f;
                while(true)
                {
                    if(canvasGroup.alpha >= 1)
                    {
                        canvasGroup.alpha = 1;
                        break;
                    }
                    canvasGroup.alpha += fadeTime * 0.02f;
                    yield return null;
                }
                break;
            case true:
                canvasGroup.alpha = 1f;
                while (true)
                {
                    
                    if (canvasGroup.alpha <= 0)
                    {
                        canvasGroup.alpha = 0;
                        break;
                    }
                    canvasGroup.alpha -= fadeTime * 0.02f;
                    yield return null;
                }
                break;
        }
    }
    
}
