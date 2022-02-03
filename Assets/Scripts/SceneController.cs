using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void startGame()
    {
        SceneManager.LoadSceneAsync("Main Scene");
    }

    public void quitGame()
    {
        print("Quit game!");
        Application.Quit(0);
    }
}
