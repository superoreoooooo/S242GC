using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScriptManager : MonoBehaviour
{
    public List<string> scripts;

    public GameObject text;
    public GameObject speechBubble;

    private TextMeshProUGUI tmpText;

    public UnityEvent onScriptEnd;

    public string nextSceneName;

    int cnt = 0;

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

    public void onScriptEnds()
    {
        FindAnyObjectByType<SceneLoader>().LoadScene(nextSceneName);
    }
}
