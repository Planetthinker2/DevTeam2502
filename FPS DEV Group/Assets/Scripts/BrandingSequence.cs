using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BrandingSequence : MonoBehaviour
{
    public Image studioLogo;   // Studio Logo
    public Image titleLogo;    // Title Logo
    public float fadeDuration = 2f;
    public float displayTime = 3f;

    void Start()
    {
        StartCoroutine(PlayBrandingSequence());
    }

    IEnumerator PlayBrandingSequence()
    {
        yield return StartCoroutine(FadeIn(studioLogo));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeIn(titleLogo));

        yield return new WaitForSeconds(displayTime);
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator FadeIn(Graphic graphic)
    {
        Color color = graphic.color;
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            graphic.color = color;
            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu"); // Skip branding sequence
        }
    }
}
