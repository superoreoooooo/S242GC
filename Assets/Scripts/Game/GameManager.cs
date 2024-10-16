using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance {
        get {
            if (instance == null) {
                instance = new GameManager();
            }
            return instance;
        }
    }

    void Awake() 
    {
        DontDestroyOnLoad(gameObject);
    }

    public void bossClear()
    {
        StartCoroutine(runScene());
    }

    private IEnumerator runScene()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<SceneLoader>().LoadScene("bossClear");
    }

    private void Update()
    {
        if (Screen.width != 1920 || Screen.height != 1080)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        }
    }

    public void quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
