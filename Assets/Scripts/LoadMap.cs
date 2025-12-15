using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Correct namespace

public class LoadMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchSceneMap()
    {
        SceneManager.LoadScene("Map"); // Correct method for loading a scene
    }
}
