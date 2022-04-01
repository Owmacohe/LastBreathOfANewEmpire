using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Febucci.UI;

public class SceneController : MonoBehaviour
{
    GameController controller;
    TextAnimator anim;

    private void Start()
    {
        controller = FindObjectOfType<GameController>();
        anim = GetComponent<TextAnimator>();

        if (anim != null)
        {
            anim.onEvent += OnEvent;
        }
    }

    private void OnDestroy()
    {
        if (anim != null)
        {
            anim.onEvent -= OnEvent;
        }
    }

    public void Load(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);

        if (sceneName.Equals("Main Scene"))
        {
            controller.playerName = FindObjectOfType<TMP_InputField>().text;
            controller.StartTutorial();
        }
    }

    public void QuitGame()
    {
        print("Quit game!");
        Application.Quit(0);
    }

    void OnEvent(string message)
    {
        switch (message)
        {
            case "start":
                controller.StartGame();
                break;
        }
    }
}
