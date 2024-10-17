using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ScriptManager : MonoBehaviour
{
    public List<string> scripts;

    public GameObject text;
    public GameObject speechBubble;

    private TextMeshProUGUI tmpText;

    public UnityEvent onScriptEnd;

    public string nextSceneName;

    int cnt = 0;

    //대사 로딩
    void Start()
    {
        tmpText = text.GetComponent<TextMeshProUGUI>();
        if (scripts == null || scripts.Count == 0)
        {
            tmpText.text = "Error!";
        } else
        {
            tmpText.text = scripts[0];
        }

        lastKeyDown = 0f;
    }

    private float lastKeyDown;

    //키보드 입력 시 대사 넘기고 페이드 효과
    void Update()
    {
        if (Input.anyKeyDown && Time.time - lastKeyDown >= 1f)
        {
            lastKeyDown = Time.time;

            cnt++;

            if (cnt >= scripts.Count)
            {
                onScriptEnd.Invoke();
                return;
            } else
            {
                speechBubble.GetComponent<FadeUI>().startFadeOut();
                text.GetComponent<UITextFade>().startManualFadeOut();
                tmpText.text = scripts[cnt];
            }
        }
    }

    //씬 전환
    public void onScriptEnds()
    {
        FindAnyObjectByType<SceneLoader>().LoadScene(nextSceneName);
    }
}
