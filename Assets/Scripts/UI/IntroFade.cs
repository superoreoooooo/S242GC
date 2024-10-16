using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroFade : MonoBehaviour
{

    private Image fadeImage;
    private float fadeDuration = 1f;
    
    //이미지 설정
    void Start()
    {
        fadeImage = GetComponent<Image>();
    }

    //인트로 페이드 아웃
    public void startgame()
    {
        StartCoroutine(FadeOut());
    }

    //페이드 아웃 코루틴 구현부
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
}
