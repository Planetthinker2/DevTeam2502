using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeEffect : MonoBehaviour
{
    public CanvasGroup fadePanel;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            fadePanel.alpha = i;
            yield return null;
        }
    }

    public void TriggerFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            fadePanel.alpha = i;
            yield return null;
        }
    }
}
