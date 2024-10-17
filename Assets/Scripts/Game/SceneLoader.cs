using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    
    //씬 로드 함수
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    //씬 로드 함수 구현부. 비동기 로드를 지원함.
    IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return new WaitForSeconds(1f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            Debug.Log("Loading progress: " + (asyncLoad.progress * 100) + "%");
            yield return null;
        }
    }
}
