using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    string sceneName;

    public void Resume()
    {
        GameManager.Instance.StateUnpause();
    }

    public void RestartLevel()
    {
        sceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadSceneAsync(sceneName);
        GameManager.Instance.StateUnpause();
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
