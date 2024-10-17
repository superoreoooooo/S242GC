using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{

    [SerializeField]
    public Image fadeImage;

    [SerializeField]
    private float fadeDuration;
    
    //페이드 인. 코루틴 사용
    public void startFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    //페이드 아웃. 코루틴 사용
    public void startFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    //페이드 아웃 (자동 페이드 인). 코루틴 사용
    public void startAutoFadeOut()
    {
        StartCoroutine(autoFadeOut());
    }

    //하드코딩됨. 이름 체크하여 특수 경우 아니면 켜질 시 페이드인 사용하게함.
    private void Start()
    {   
        if (gameObject.name != "SpeechBubble") StartCoroutine(a());
    }

    //페이드인 코루틴을 실행하는 코루틴 구현부. 특수 경우에만 사용됨
    private IEnumerator a()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeIn());
    }

    //페이드 인 코루틴 구현부.
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
    }

    //페이드 아웃 코루틴 구현부.
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
    }

    //페이드 아웃 코루틴 구현부. 이후 자동 페이드 인
    public IEnumerator autoFadeOut()
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

        startFadeIn();
    }
}
