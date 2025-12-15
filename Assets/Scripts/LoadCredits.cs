using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Correct namespace

public class LoadCredits : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchScene()
    {
        SceneManager.LoadScene("Credits"); // Correct method for loading a scene
    }
}
