using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    //싱글톤 패턴 활용한 게임 매니저
    public static GameManager Instance {
        get {
            if (instance == null) {
                instance = new GameManager();
            }
            return instance;
        }
    }

    //오브젝트 로드 시 DontDestroyOnLoad를 이용하여 씬 전환에도 오브젝트가 유지되게 함
    void Awake() 
    {
        DontDestroyOnLoad(gameObject);
    }

    //보스 클리어 시 이벤트에 의해 호출됨
    public void bossClear()
    {
        StartCoroutine(runScene());
    }

    //보스 클리어 코루틴 구현부    
    private IEnumerator runScene()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<SceneLoader>().LoadScene("bossClear");
    }

    //화면 해상도 고정
    private void Update()
    {
        if (Screen.width != 1920 || Screen.height != 1080)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        }
    }
    
    //게임 종료 관련
    public void quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
