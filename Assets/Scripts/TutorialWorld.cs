using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Correct namespace

public class TutorialWorld : MonoBehaviour
{

    public void LoadTut()
    {
        SceneManager.LoadScene("Tut"); // Correct method for loading a scene
    }
}
