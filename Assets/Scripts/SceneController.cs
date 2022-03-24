using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneController : MonoBehaviour
{
    public void load(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);

        if (sceneName.Equals("Main Scene"))
        {
            GameController controller = FindObjectOfType<GameController>();

            controller.playerName = FindObjectOfType<TMP_InputField>().text;
            controller.startGame();
        }
    }

    public void quitGame()
    {
        print("Quit game!");
        Application.Quit(0);
    }
}
