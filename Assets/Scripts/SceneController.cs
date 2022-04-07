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
        if (sceneName.Equals("Main Scene"))
        {
            TMP_InputField temp = FindObjectOfType<TMP_InputField>();

            if (temp.text.Length > 0 && temp.text[0] != ' ' && temp.text[temp.text.Length - 1] != ' ')
            {
                SceneManager.LoadSceneAsync(sceneName);
                controller.StartTutorial();   
            }
        }
        else
        {
            SceneManager.LoadSceneAsync(sceneName);
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
