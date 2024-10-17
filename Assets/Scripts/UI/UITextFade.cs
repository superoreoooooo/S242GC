using System.Collections;
using TMPro;
using UnityEngine;

public class UITextFade : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI fadeText;

    [SerializeField]
    private float fadeDuration;

    //페이드 인. 코루틴 사용
    public void startFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    //페이드 아웃 (자동 페이드 인). 코루틴 사용
    public void startFadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    //페이드 아웃 (수동 페이드 인). 코루틴 사용
    public void startManualFadeOut()
    {
        StartCoroutine(ManualFadeOut());
    }

    //페이드 인 코루틴 구현부.
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

    //페이드 아웃 코루틴 구현부. 이후 자동 페이드 인
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

    //페이드 아웃 코루틴 구현부.
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
