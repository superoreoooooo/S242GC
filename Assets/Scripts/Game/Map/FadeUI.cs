using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{

    [SerializeField]
    public Image fadeImage;

    [SerializeField]
    private float fadeDuration;

    private bool isDarkened = false;

    public void startFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void startFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    private void Start()
    {   
        StartCoroutine(a());
    }

    private void Update()
    {
    }

    private IEnumerator a()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        color.a = 1f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
        isDarkened = false;
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        color.a = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
        isDarkened = true;
    }
}
