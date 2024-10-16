using System.Collections;
using System.Collections.Generic;
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
}
