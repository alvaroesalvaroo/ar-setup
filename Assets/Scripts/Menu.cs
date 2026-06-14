using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void ChangeToAR()
    {
        StartCoroutine(LoadAsyncScene("ARScene"));
    }

    public void ChangeToMenu()
    {
        StartCoroutine(LoadAsyncScene("MenuScene"));
    }
    IEnumerator LoadAsyncScene(String sceneName)
    {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
