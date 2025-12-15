using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadWorld3 : MonoBehaviour
{
    public GameObject World3Button;
    public GameObject locks;

    private void Start()
    {
        bool unlocked = GameData.worldCount >= 3;

        World3Button.SetActive(unlocked);
        locks.SetActive(!unlocked);
    }

    public void World3Load()
    {
        if (GameData.worldCount >= 3)
        {
            SceneManager.LoadScene("World 3");
        }
    }
}
