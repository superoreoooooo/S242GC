using System.Collections;
using TMPro;
using UnityEngine;

public class UITextFade : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI fadeText;

    [SerializeField]
    private float fadeDuration;

    public void startFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void startFadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    public void startManualFadeOut()
    {
        StartCoroutine(ManualFadeOut());
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeText.color;
        color.a = 1f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeText.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeText.color = color;
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeText.color;
        color.a = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeText.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeText.color = color;

        
        yield return new WaitForSeconds(1f);

        startFadeIn();
    }

    public IEnumerator ManualFadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeText.color;
        color.a = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeText.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeText.color = color;
    }
}
