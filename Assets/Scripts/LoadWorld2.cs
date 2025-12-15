using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadWorld2 : MonoBehaviour
{
    public GameObject World2Button;
    public GameObject locks;

    private void Start()
    {
        bool unlocked = GameData.worldCount >= 2;

        World2Button.SetActive(unlocked);
        locks.SetActive(!unlocked);
    }

    public void World2Load()
    {
        if (GameData.worldCount >= 2)
        {
            SceneManager.LoadScene("World 2");
        }
    }
}
