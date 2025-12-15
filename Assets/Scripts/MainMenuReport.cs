using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Correct namespace

public class MainMenuReport : MonoBehaviour
{

    public void SwitchSceneMain()
    {
        SceneManager.LoadScene("Main Menu"); // Correct method for loading a scene
    }
}
